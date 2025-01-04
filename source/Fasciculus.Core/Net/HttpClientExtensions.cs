using Fasciculus.Threading;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Fasciculus.Net
{
    /// <summary>
    /// Extensions for <see cref="HttpClient"/>
    /// </summary>
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Returns the response headers of a "HEAD" request.
        /// </summary>
        public static async Task<HttpContentHeaders> HeadAsync(this HttpClient httpClient, Uri? requestUri)
        {
            using HttpRequestMessage request = new(HttpMethod.Head, requestUri);
            using HttpResponseMessage response = await httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            return response.Content.Headers;
        }

        /// <summary>
        /// Returns the response headers of a "HEAD" request.
        /// </summary>
        public static HttpContentHeaders Head(this HttpClient httpClient, Uri? requestUri)
            => Tasks.Wait(httpClient.HeadAsync(requestUri));
    }
}
