using Fasciculus.Maui.Services;
using Fasciculus.Net;
using Fasciculus.Support;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Services
{
    public interface IEsiHttp
    {
        public Task<string?> GetSingle(string uri);
        public Task<Tuple<string[], bool>> GetPaged(string uri, IAccumulatingLongProgress progress);
    }

    public class EsiHttp : IEsiHttp
    {
        public const string UserAgentKey = "EsiUserAgent";
        private static readonly Uri BaseUri = new("https://esi.evetech.net/latest/");

        private readonly HttpClient httpClient;
        private readonly IExceptions exceptions;

        private readonly TaskSafeMutex mutex = new();

        public EsiHttp(IHttpClientPool httpClientPool, [FromKeyedServices(UserAgentKey)] string userAgent, IExceptions exceptions)
        {
            httpClient = CreateHttpClient(httpClientPool, userAgent);
            this.exceptions = exceptions;
        }

        public async Task<string?> GetSingle(string uri)
        {
            using Locker locker = Locker.Lock(mutex);

            Tuple<string, int>? textAndPages = await Get(uri);

            return textAndPages?.Item1;
        }

        public async Task<Tuple<string[], bool>> GetPaged(string uri, IAccumulatingLongProgress progress)
        {
            using Locker locker = Locker.Lock(mutex);

            List<string> result = [];
            bool success = true;
            int page = 1;
            int pages = 1;

            progress.Begin(1);

            while (page <= pages)
            {
                string uriWithPage = $"{uri}&page={page}";
                Tuple<string, int>? textAndPages = await Get(uriWithPage);

                if (textAndPages is null)
                {
                    success = false;
                    break;
                }

                result.Add(textAndPages.Item1);

                if (page == 1)
                {
                    pages = textAndPages.Item2;
                    progress.Begin(pages);
                }
                else
                {
                    pages = Math.Min(pages, textAndPages.Item2);
                }

                ++page;
                progress.Report(1);
            }

            progress.End();

            return Tuple.Create(result.ToArray(), success);
        }

        private async Task<Tuple<string, int>?> Get(string uri)
        {
            try
            {
                using HttpRequestMessage request = new(HttpMethod.Get, uri);
                using HttpResponseMessage response = await httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                string text = await response.Content.ReadAsStringAsync();
                int pages = GetPages(response.Headers);

                return Tuple.Create(text, pages);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

            return null;
        }

        private static int GetPages(HttpResponseHeaders headers)
        {
            if (headers.Contains("x-pages"))
            {
                string pagesString = headers.GetValues("x-pages").FirstOrDefault() ?? "1";

                return int.TryParse(pagesString, out int pages) ? pages : 1;
            }

            return 1;
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
}
