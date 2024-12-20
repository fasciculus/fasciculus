using Fasciculus.Steam.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FasciculusSteamConfiguration
    {
        public static IServiceCollection AddSteam(this IServiceCollection services)
        {
            services.TryAddSingleton<ISteamApplications, SteamApplications>();

            return services;
        }
    }
}
