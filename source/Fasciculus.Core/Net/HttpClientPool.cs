using Fasciculus.Threading;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Fasciculus.Net
{
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
}
