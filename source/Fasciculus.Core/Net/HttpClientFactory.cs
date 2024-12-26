using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Fasciculus.Net
{
    /// <summary>
    /// Options used by <see cref="HttpClientFactory"/>.
    /// </summary>
    public class HttpClientOptions : HttpClientHandlerOptions
    {
    }

    /// <summary>
    /// Factory to create <see cref="HttpClient"/>s.
    /// </summary>
    public static class HttpClientFactory
    {
        /// <summary>
        /// Creates a <see cref="HttpClientHandler"/> and <see cref="HttpClient"/> using the given <paramref name="options"/>.
        /// </summary>
        public static HttpClient Create(HttpClientOptions? options)
        {
            HttpClientHandler handler = HttpClientHandlerFactory.Create(options);

            return Create(handler, options);
        }

        /// <summary>
        /// Creates a <see cref="HttpClient"/> using the given <paramref name="handler"/> and <paramref name="options"/>.
        /// </summary>
        public static HttpClient Create(HttpClientHandler handler, HttpClientOptions? options)
        {
            HttpClient client = new(handler);

            if ((handler.AutomaticDecompression & DecompressionMethods.GZip) != 0)
            {
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            }

            if ((handler.AutomaticDecompression & DecompressionMethods.Deflate) != 0)
            {
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            }

            return client;
        }
    }
}
