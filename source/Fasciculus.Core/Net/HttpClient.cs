using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Fasciculus.Net
{
    public interface IHttpClientHandlers
    {
        public HttpClientHandler this[Uri uri] { get; }
    }

    public class HttpClientHandlers : IHttpClientHandlers
    {
        private readonly Dictionary<string, HttpClientHandler> httpClientHandlers = [];
        private readonly TaskSafeMutex httpClientHandlersMutex = new();

        public HttpClientHandler this[Uri uri]
        {
            get
            {
                using Locker locker = Locker.Lock(httpClientHandlersMutex);

                if (httpClientHandlers.TryGetValue(uri.Host, out HttpClientHandler? httpClientHandler))
                {
                    return httpClientHandler;
                }

                httpClientHandler = new();
                httpClientHandlers[uri.Host] = httpClientHandler;

                return httpClientHandler;
            }
        }
    }

    public interface IHttpClientPool
    {
        public HttpClient this[Uri uri] { get; }
    }

    public class HttpClientPool : IHttpClientPool
    {
        private readonly IHttpClientHandlers httpClientHandlers;

        private readonly Dictionary<string, HttpClient> httpClients = [];
        private readonly TaskSafeMutex httpClientsMutex = new();

        public HttpClientPool(IHttpClientHandlers httpClientHandlers)
        {
            this.httpClientHandlers = httpClientHandlers;
        }

        public HttpClient this[Uri uri]
        {
            get
            {
                using Locker locker = Locker.Lock(httpClientsMutex);

                if (httpClients.TryGetValue(uri.Host, out HttpClient? httpClient))
                {
                    return httpClient;
                }

                HttpClientHandler httpClientHandler = httpClientHandlers[uri];

                httpClient = new(httpClientHandler);
                httpClients[uri.Host] = httpClient;

                return httpClient;
            }
        }
    }

    public interface IDownloader
    {
        public FileInfo Download(Uri uri, FileInfo target);
    }

    public class Downloader : IDownloader
    {
        private readonly IHttpClientPool httpClientPool;

        public Downloader(IHttpClientPool httpClientPool)
        {
            this.httpClientPool = httpClientPool;
        }

        public FileInfo Download(Uri uri, FileInfo destination)
        {
            HttpClient httpClient = httpClientPool[uri];
            HttpRequestMessage httpRequest = new(HttpMethod.Get, uri);

            if (File.Exists(destination.FullName))
            {
                httpRequest.Headers.IfModifiedSince = destination.LastWriteTimeUtc;
            }

            HttpResponseMessage httpResponse = httpClient.SendAsync(httpRequest).Run();

            if (httpResponse.StatusCode == System.Net.HttpStatusCode.NotModified)
            {
                return destination;
            }

            httpResponse.EnsureSuccessStatusCode();

            byte[] bytes = httpResponse.Content.ReadAsByteArrayAsync().Run();

            destination.DeleteIfExists();
            destination.WriteAllBytes(bytes);

            return new(destination.FullName);
        }
    }

    public static class HttpClientServices
    {
        public static IServiceCollection AddHttpClientHandlers(this IServiceCollection services)
        {
            services.TryAddSingleton<IHttpClientHandlers, HttpClientHandlers>();

            return services;
        }

        public static HostApplicationBuilder UseHttpClientHandlers(this HostApplicationBuilder builder)
        {
            builder.Services.AddHttpClientHandlers();

            return builder;
        }

        public static IServiceCollection AddHttpClientPool(this IServiceCollection services)
        {
            services.AddHttpClientHandlers();
            services.TryAddSingleton<IHttpClientPool, HttpClientPool>();

            return services;
        }

        public static HostApplicationBuilder UseHttpClientPool(this HostApplicationBuilder builder)
        {
            builder.Services.AddHttpClientPool();

            return builder;
        }

        public static IServiceCollection AddDownloader(this IServiceCollection services)
        {
            services.AddHttpClientPool();
            services.TryAddSingleton<IDownloader, Downloader>();

            return services;
        }

        public static HostApplicationBuilder UseDownloader(this HostApplicationBuilder builder)
        {
            builder.Services.AddDownloader();

            return builder;
        }
    }
}

