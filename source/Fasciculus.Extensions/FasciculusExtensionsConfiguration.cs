using Fasciculus.Net;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Utilities to add Fasciculus.Extensions services to a service collection.
    /// </summary>
    public static class FasciculusExtensionsConfiguration
    {
        /// <summary>
        /// Adds <see cref="IHttpClientHandlers"/> implementation, including it's dependencies.
        /// </summary>
        public static IServiceCollection AddHttpClientHandlers(this IServiceCollection services)
        {
            services.TryAddSingleton<IHttpClientHandlers, HttpClientHandlers>();

            return services;
        }

        /// <summary>
        /// Adds <see cref="IHttpClientPool"/> implementation, including it's dependencies.
        /// </summary>
        public static IServiceCollection AddHttpClientPool(this IServiceCollection services)
        {
            services.AddHttpClientHandlers();

            services.TryAddSingleton<IHttpClientPool, HttpClientPool>();

            return services;
        }

        /// <summary>
        /// Adds <see cref="IDownloader"/> implementation, including it's dependencies.
        /// </summary>
        public static IServiceCollection AddDownloader(this IServiceCollection services)
        {
            services.AddHttpClientPool();

            services.TryAddSingleton<IDownloader, Downloader>();

            return services;
        }
    }
}
