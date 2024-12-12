﻿using Fasciculus.Eve.Services;
using Fasciculus.IO;
using Fasciculus.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Maui.Hosting;

namespace Fasciculus.Eve
{
    public static class EveCoreConfiguration
    {
        public static MauiAppBuilder UseEveCore(this MauiAppBuilder builder, string esiUserAgent)
        {
            IServiceCollection services = builder.Services;

            services.AddSpecialDirectories();
            services.AddEmbeddedResources();
            services.AddHttpClientPool();

            services.TryAddSingleton<IEveFileSystem, EveFileSystem>();

            services.TryAddSingleton<IEveResourcesProgress, EveResourcesProgress>();
            services.TryAddSingleton<IEveResources, EveResources>();

            services.TryAddKeyedSingleton(EsiClient.UserAgentKey, esiUserAgent);
            services.TryAddSingleton<IEsiCacheFiles, EsiCacheFiles>();
            services.TryAddSingleton<IEsiCache, EsiCache>();
            services.TryAddSingleton<IEsiClient, EsiClient>();

            services.TryAddSingleton<IPlanets, Planets>();
            services.TryAddSingleton<ITrades, Trades>();

            return builder;
        }
    }
}