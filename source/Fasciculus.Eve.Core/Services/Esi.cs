using Fasciculus.Eve.Models;
using Fasciculus.Net;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Services
{
    public interface IEsiCacheFiles
    {
        public FileInfo MarketPrices { get; }
    }

    public class EsiCacheFiles : IEsiCacheFiles
    {
        private readonly DirectoryInfo cache;
        private readonly DirectoryInfo market;

        public FileInfo MarketPrices => market.File("Prices");

        public EsiCacheFiles(IEveFileSystem eveFileSystem)
        {
            cache = eveFileSystem.Documents.Combine("EsiCache").CreateIfNotExists();
            market = cache.Combine("Market").CreateIfNotExists();
        }
    }

    public interface IEsiCache
    {
        public EveMarketPrices? MarketPrices { get; set; }
    }

    public class EsiCache : IEsiCache
    {
        public static readonly TimeSpan MarketPricesMaxAge = TimeSpan.FromSeconds(3600);

        private readonly IEsiCacheFiles files;

        private readonly EveTypes types;

        public EveMarketPrices? MarketPrices
        {
            get => Read(files.MarketPrices, MarketPricesMaxAge, s => new EveMarketPrices(s, types));
            set { if (value is not null) files.MarketPrices.Write(value.Write); }
        }

        public EsiCache(IEsiCacheFiles files, IEveResources resources)
        {
            this.files = files;

            types = Tasks.Wait(resources.Data).Types;
        }

        private static T? Read<T>(FileInfo file, TimeSpan maxAge, Func<Stream, T> read)
            where T : notnull
        {
            T? result = default;

            if (file.Exists && file.IsNewerThan(DateTime.UtcNow - maxAge))
            {
                result = file.Read(read);
            }

            return result;
        }
    }

    public interface IEsiClient
    {
        public Task<EveMarketPrices> MarketPrices { get; }
    }

    public class EsiClient : IEsiClient
    {
        private static readonly Uri BaseUri = new("https://esi.evetech.net/latest/");

        private readonly HttpClient httpClient;
        private readonly IEsiCache cache;

        private readonly EveTypes types;

        public Task<EveMarketPrices> MarketPrices => GetEveMarketPricesAsync();

        public EsiClient(IHttpClientPool httpClientPool, [FromKeyedServices("EsiUserAgent")] string userAgent, IEsiCache cache,
            IEveResources resources)
        {
            httpClient = GetHttpClient(httpClientPool, userAgent);
            this.cache = cache;

            types = Tasks.Wait(resources.Data).Types;
        }

        private async Task<EveMarketPrices> GetEveMarketPricesAsync()
        {
            EveMarketPrices? result = cache.MarketPrices;

            if (result == null)
            {
                HttpRequestMessage request = new(HttpMethod.Get, "markets/prices");
                HttpResponseMessage response = await httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                EsiMarketPrice[] esiMarketPrices = JsonSerializer.Deserialize<EsiMarketPrice[]>(json)!;
                Dictionary<int, double> dictionary = esiMarketPrices.ToDictionary(x => x.TypeId, x => x.AveragePrice);
                EveMarketPrices.Data data = new(dictionary);

                result = new(data, types);
                cache.MarketPrices = result;
            }

            return result;
        }

        private static HttpClient GetHttpClient(IHttpClientPool httpClientPool, string userAgent)
        {
            HttpClient httpClient = httpClientPool[BaseUri];

            httpClient.BaseAddress = BaseUri;
            httpClient.DefaultRequestHeaders.Add("X-User-Agent", userAgent);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));

            return httpClient;
        }
    }

    public static class EsiServices
    {
        public static IServiceCollection AddEsi(this IServiceCollection services)
        {
            services.AddEveFileSystem();
            services.AddEveResources();
            services.AddHttpClientPool();

            services.TryAddSingleton<IEsiCacheFiles, EsiCacheFiles>();
            services.TryAddSingleton<IEsiCache, EsiCache>();
            services.TryAddSingleton<IEsiClient, EsiClient>();

            return services;
        }
    }
}
