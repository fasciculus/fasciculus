﻿using Fasciculus.IO;
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
        /// Adds <see cref="ISpecialPaths"/> implementation.
        /// </summary>
        public static IServiceCollection AddSpecialPaths(this IServiceCollection services)
        {
            services.TryAddSingleton<ISpecialPaths, SpecialPaths>();

            return services;
        }

        /// <summary>
        /// Adds <see cref="ISpecialDirectories"/> implementation, including it's dependencies.
        /// </summary>
        public static IServiceCollection AddSpecialDirectories(this IServiceCollection services)
        {
            services.AddSpecialPaths();

            services.TryAddSingleton<ISpecialDirectories, SpecialDirectories>();

            return services;
        }

        /// <summary>
        /// Adds <see cref="ICompression"/> implementation.
        /// </summary>
        public static IServiceCollection AddCompression(this IServiceCollection services)
        {
            services.TryAddSingleton<ICompression, Compression>();

            return services;
        }

        /// <summary>
        /// Adds <see cref="IEmbeddedResources"/> implementation, including it's dependencies.
        /// </summary>
        public static IServiceCollection AddEmbeddedResources(this IServiceCollection services)
        {
            services.AddCompression();

            services.TryAddSingleton<IEmbeddedResources, EmbeddedResources>();

            return services;
        }

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
