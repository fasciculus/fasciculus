using System.Net;
using System.Net.Http;

namespace Fasciculus.Net
{
    /// <summary>
    /// Factory to create <see cref="HttpClientHandler"/>s.
    /// </summary>
    public static class HttpHandlerFactory
    {
        /// <summary>
        /// Creates a new <see cref="HttpClientHandler"/> with the given <paramref name="options"/>.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static HttpClientHandler CreateHandler(HttpHandlerOptions? options)
        {
            return new()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
        }
    }
}
