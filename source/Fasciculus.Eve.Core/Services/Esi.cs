using Fasciculus.Eve.Models;
using Fasciculus.Net;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
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

        public FileInfo MarketOrders(EveRegion region, bool buy);
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

        public FileInfo MarketOrders(EveRegion region, bool buy)
        {
            string buyOrSell = buy ? "buy" : "sell";
            DirectoryInfo directory = market.Combine("Orders", buyOrSell).CreateIfNotExists();

            return directory.File(region.Id.ToString());
        }
    }

    public interface IEsiCache
    {
        public EveMarketPrices? MarketPrices { get; set; }

        public EveMarketOrders? GetMarketOrders(EveRegion region, bool buy);
        public void SetMarketOrders(EveRegion region, bool buy, EveMarketOrders marketOrders);
    }

    public class EsiCache : IEsiCache
    {
#if DEBUG
        public static readonly TimeSpan MarketPricesMaxAge = TimeSpan.FromSeconds(360000);
        public static readonly TimeSpan MarketOrdersMaxAge = TimeSpan.FromSeconds(360000);
#else
        public static readonly TimeSpan MarketPricesMaxAge = TimeSpan.FromSeconds(3600);
        public static readonly TimeSpan MarketOrdersMaxAge = TimeSpan.FromSeconds(300);
#endif

        private readonly IEsiCacheFiles files;

        private readonly EveTypes types;

        public EveMarketPrices? MarketPrices
        {
            get => Read(files.MarketPrices, MarketPricesMaxAge, s => new EveMarketPrices(s, types));
            set { if (value is not null) Write(files.MarketPrices, value.Write); }
        }

        public EsiCache(IEsiCacheFiles files, IEveResources resources)
        {
            this.files = files;

            types = Tasks.Wait(resources.Data).Types;
        }

        public EveMarketOrders? GetMarketOrders(EveRegion region, bool buy)
            => Read(files.MarketOrders(region, buy), MarketOrdersMaxAge, s => new EveMarketOrders(s));

        public void SetMarketOrders(EveRegion region, bool buy, EveMarketOrders marketOrders)
            => Write(files.MarketOrders(region, buy), marketOrders.Write);

        private static T? Read<T>(FileInfo file, TimeSpan maxAge, Func<Stream, T> read)
            where T : notnull
        {
            using Locker locker = NamedTaskSafeMutexes.Lock(file.FullName);

            T? result = default;

            if (file.Exists && file.IsNewerThan(DateTime.UtcNow - maxAge))
            {
                result = file.Read(read);
            }

            return result;
        }

        private static void Write(FileInfo file, Action<Stream> write)
        {
            using Locker locker = NamedTaskSafeMutexes.Lock(file.FullName);

            file.Write(write);
        }
    }

    public interface IEsiClient
    {
        public Task<EveMarketPrices> MarketPrices { get; }

        public Task<EveMarketOrders> GetMarketOrdersAsync(EveRegion region, bool buy);
    }

    public class EsiClient : IEsiClient
    {
        private static readonly Uri BaseUri = new("https://esi.evetech.net/latest/");

        private readonly HttpClient httpClient;
        private readonly IEsiCache cache;

        private readonly EveTypes types;
        private readonly EveMoonStations stations;

        private readonly ILogger logger;

        public Task<EveMarketPrices> MarketPrices => GetEveMarketPricesAsync();

        public EsiClient(IHttpClientPool httpClientPool, IEsiCache cache, IEveResources resources, ILogger<EsiClient> logger,
            [FromKeyedServices("EsiUserAgent")] string userAgent)
        {
            httpClient = CreateHttpClient(httpClientPool, userAgent);
            this.cache = cache;

            types = Tasks.Wait(resources.Data).Types;
            stations = Tasks.Wait(resources.Universe).NpcStations;

            this.logger = logger;
        }

        private async Task<EveMarketPrices> GetEveMarketPricesAsync()
        {
            using Locker locker = NamedTaskSafeMutexes.Lock("MarketPrices");

            EveMarketPrices? result = cache.MarketPrices;

            if (result is null)
            {
                try
                {
                    HttpRequestMessage request = new(HttpMethod.Get, "markets/prices");
                    HttpResponseMessage response = await httpClient.SendAsync(request);

                    response.EnsureSuccessStatusCode();

                    string json = await response.Content.ReadAsStringAsync();
                    EsiMarketPrice[] esiMarketPrices = JsonSerializer.Deserialize<EsiMarketPrice[]>(json) ?? [];

                    Dictionary<int, double> dictionary = esiMarketPrices
                        .Where(x => x.AveragePrice > 0)
                        .ToDictionary(x => x.TypeId, x => x.AveragePrice);

                    EveMarketPrices.Data data = new(dictionary);

                    result = new(data, types);
                }
                catch (Exception ex)
                {
                    logger.LogError("{message}", ex.Message);
                    result = EveMarketPrices.Empty(types);
                }

                cache.MarketPrices = result;
            }

            return result;
        }

        public async Task<EveMarketOrders> GetMarketOrdersAsync(EveRegion region, bool buy)
        {
            using Locker locker = NamedTaskSafeMutexes.Lock($"MarketOrders_{region.Id}_{buy}");

            EveMarketOrders? result = cache.GetMarketOrders(region, buy);

            if (result is null)
            {
                IEnumerable<EsiMarketOrder> esiOrders = [];

                string orderType = buy ? "buy" : "sell";
                int pageCount = 1;
                int page = 0;

                while (page < pageCount)
                {
                    ++page;

                    try
                    {
                        string uri = $"markets/{region.Id}/orders/?order_type={orderType}&page={page}";
                        HttpRequestMessage request = new(HttpMethod.Get, uri);
                        HttpResponseMessage response = await httpClient.SendAsync(request);

                        response.EnsureSuccessStatusCode();

                        string json = await response.Content.ReadAsStringAsync();
                        EsiMarketOrder[] pageOrders = JsonSerializer.Deserialize<EsiMarketOrder[]>(json) ?? [];

                        esiOrders = esiOrders.Concat(pageOrders);

                        if (response.Headers.Contains("x-pages"))
                        {
                            IEnumerable<string> values = response.Headers.GetValues("x-pages");
                            string value = values.FirstOrDefault() ?? "1";

                            if (int.TryParse(value, out int newPageCount))
                            {
                                pageCount = newPageCount;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError("{message}", ex.Message);
                        break;
                    }
                }

                IEnumerable<EveMarketOrder.Data> orders = esiOrders.Select(Convert).NotNull();
                EveMarketOrders.Data data = new(orders);

                result = new(data);
                cache.SetMarketOrders(region, buy, result);
            }

            return result;
        }

        private EveMarketOrder.Data? Convert(EsiMarketOrder order)
        {
            EveMarketOrder.Data? result = null;
            long location = order.LocationId;

            if (stations.Contains(location))
            {
                result = new(order.Type, location, order.IsBuyOrder, order.Price, order.Quantity);
            }

            return result;
        }

        private static HttpClient CreateHttpClient(IHttpClientPool httpClientPool, string userAgent)
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
