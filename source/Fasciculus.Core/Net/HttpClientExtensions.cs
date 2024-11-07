using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace System.Net.Http
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpContentHeaders> HeadAsync(this HttpClient httpClient, Uri? requestUri)
        {
            using HttpRequestMessage request = new(HttpMethod.Head, requestUri);
            using HttpResponseMessage response = await httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            return response.Content.Headers;
        }

        public static HttpContentHeaders Head(this HttpClient httpClient, Uri? requestUri)
            => httpClient.HeadAsync(requestUri).Run();

        public static byte[] GetByteArray(this HttpClient httpClient, Uri? requestUri)
            => httpClient.GetByteArrayAsync(requestUri).Run();
    }
}
