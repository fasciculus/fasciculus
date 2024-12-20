using Fasciculus.IO;
using Fasciculus.Net;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FasciculusExtensionsConfiguration
    {
        public static IServiceCollection AddSpecialPaths(this IServiceCollection services)
        {
            services.TryAddSingleton<ISpecialPaths, SpecialPaths>();

            return services;
        }

        public static IServiceCollection AddSpecialDirectories(this IServiceCollection services)
        {
            services.AddSpecialPaths();

            services.TryAddSingleton<ISpecialDirectories, SpecialDirectories>();

            return services;
        }

        public static IServiceCollection AddCompression(this IServiceCollection services)
        {
            services.TryAddSingleton<ICompression, Compression>();

            return services;
        }

        public static IServiceCollection AddEmbeddedResources(this IServiceCollection services)
        {
            services.AddCompression();

            services.TryAddSingleton<IEmbeddedResources, EmbeddedResources>();

            return services;
        }

        public static IServiceCollection AddHttpClientHandlers(this IServiceCollection services)
        {
            services.TryAddSingleton<IHttpClientHandlers, HttpClientHandlers>();

            return services;
        }

        public static IServiceCollection AddHttpClientPool(this IServiceCollection services)
        {
            services.AddHttpClientHandlers();

            services.TryAddSingleton<IHttpClientPool, HttpClientPool>();

            return services;
        }

        public static IServiceCollection AddDownloader(this IServiceCollection services)
        {
            services.AddHttpClientPool();

            services.TryAddSingleton<IDownloader, Downloader>();

            return services;
        }
    }
}
