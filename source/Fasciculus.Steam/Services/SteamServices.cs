using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Steam.Services
{
    public static class SteamServices
    {
        public static IServiceCollection AddSteam(this IServiceCollection services)
        {
            services.TryAddSingleton<ISteamApplications, SteamApplications>();

            return services;
        }
    }
}
