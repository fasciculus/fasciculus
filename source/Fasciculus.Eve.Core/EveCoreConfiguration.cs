using Fasciculus.Eve.Models;
using Fasciculus.Eve.Services;
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

            services.TryAddSingleton<IEveFileSystem, EveFileSystem>();
            services.TryAddSingleton<IEveSettings, EveSettings>();

            services.TryAddSingleton<IEveResourcesProgress, EveResourcesProgress>();
            services.TryAddSingleton<IEveResources, EveResources>();
            services.TryAddSingleton<IDataProvider, DataProvider>();
            services.TryAddSingleton<IUniverseProvider, UniverseProvider>();
            services.TryAddSingleton<INavigationProvider, NavigationProvider>();
            services.TryAddSingleton<IEveProvider, EveProvider>();

            services.TryAddSingleton<SkillManager>();
            services.TryAddSingleton<ISkillManager>(x => x.GetRequiredService<SkillManager>());
            services.TryAddSingleton<ISkillInfoProvider>(x => x.GetRequiredService<SkillManager>());
            services.TryAddSingleton<IMutableSkillProvider>(x => x.GetRequiredService<SkillManager>());
            services.TryAddSingleton<ISkillProvider>(x => x.GetRequiredService<SkillManager>());

            services.TryAddKeyedSingleton(EsiHttp.UserAgentKey, esiUserAgent);
            services.TryAddSingleton<IEsiHttp, EsiHttp>();
            services.TryAddSingleton<IEsiCache, EsiCache>();
            services.TryAddSingleton<IEsiClient, EsiClient>();

            services.TryAddSingleton<IMarket, Market>();

            services.TryAddSingleton<IPlanetSchematics, PlanetSchematics>();
            services.TryAddSingleton<IPlanetBaseCosts, PlanetBaseCosts>();
            services.TryAddSingleton<IPlanetChains, PlanetChains>();
            services.TryAddSingleton<IPlanets, Planets>();

            services.TryAddSingleton<IIndustry, Industry>();
            services.TryAddSingleton<ITrades, Trades>();

            return builder;
        }
    }
}
