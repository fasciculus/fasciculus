using Fasciculus.Threading;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

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
}
