using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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

                httpClientHandler = CreateHttpClientHandler();
                httpClientHandlers[uri.Host] = httpClientHandler;

                return httpClientHandler;
            }
        }

        public HttpClientHandler CreateHttpClientHandler()
        {
            return new()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
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

                httpClient = CreateHttpClient(httpClientHandler);
                httpClients[uri.Host] = httpClient;

                return httpClient;
            }
        }

        public HttpClient CreateHttpClient(HttpClientHandler httpClientHandler)
        {
            HttpClient httpClient = new(httpClientHandler);

            if ((httpClientHandler.AutomaticDecompression & DecompressionMethods.GZip) != 0)
            {
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            }

            if ((httpClientHandler.AutomaticDecompression & DecompressionMethods.Deflate) != 0)
            {
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            }

            return httpClient;
        }
    }

    public interface IDownloader
    {
        public FileInfo Download(Uri uri, FileInfo destination);
        public FileInfo Download(Uri uri, FileInfo destination, out bool notModified);
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
            return Download(uri, destination, out bool _);
        }

        public FileInfo Download(Uri uri, FileInfo destination, out bool notModified)
        {
            HttpClient httpClient = httpClientPool[uri];
            HttpRequestMessage httpRequest = new(HttpMethod.Get, uri);

            notModified = false;

            if (File.Exists(destination.FullName))
            {
                httpRequest.Headers.IfModifiedSince = destination.LastWriteTimeUtc;
            }

            HttpResponseMessage httpResponse = httpClient.SendAsync(httpRequest).Run();

            if (httpResponse.StatusCode == HttpStatusCode.NotModified)
            {
                notModified = true;
            }
            else
            {
                httpResponse.EnsureSuccessStatusCode();

                byte[] bytes = httpResponse.Content.ReadAsByteArrayAsync().Run();

                destination.DeleteIfExists();
                destination.WriteAllBytes(bytes);
                destination = new(destination.FullName);
            }

            return destination;
        }
    }

    public static class HttpClientServices
    {
        public static IServiceCollection AddHttpClientHandlers(this IServiceCollection services)
        {
            services.TryAddSingleton<IHttpClientHandlers, HttpClientHandlers>();

            return services;
        }

        public static IServiceCollection AddHttpClientPool(this IServiceCollection services)
        {
            services.AddHttpClientHandlers();
            services.TryAddSingleton<IHttpClientPool, HttpClientPool>();

            return services;
        }

        public static IServiceCollection AddDownloader(this IServiceCollection services)
        {
            services.AddHttpClientPool();
            services.TryAddSingleton<IDownloader, Downloader>();

            return services;
        }
    }
}

