using System.Net;
using System.Net.Http;

namespace Fasciculus.Net
{
    /// <summary>
    /// Options used for <see cref="HttpClientHandlerFactory.Create(HttpClientHandlerOptions?)"/>.
    /// </summary>
    public class HttpClientHandlerOptions
    {
    }

    /// <summary>
    /// Factory to create <see cref="HttpClientHandler"/>s.
    /// </summary>
    public static class HttpClientHandlerFactory
    {
        /// <summary>
        /// Creates a new <see cref="HttpClientHandler"/> with the given <paramref name="options"/>.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static HttpClientHandler Create(HttpClientHandlerOptions? options)
        {
            return new()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
        }
    }
}
