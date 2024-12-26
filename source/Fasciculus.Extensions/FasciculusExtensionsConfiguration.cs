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
        /// Adds <see cref="IDownloader"/> implementation, including it's dependencies.
        /// </summary>
        public static IServiceCollection AddDownloader(this IServiceCollection services)
        {
            services.TryAddSingleton<IDownloader, Downloader>();

            return services;
        }
    }
}
