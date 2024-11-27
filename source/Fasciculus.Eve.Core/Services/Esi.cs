using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Services
{
    public interface IEsiCache
    {
    }

    public class EsiCache : IEsiCache
    {
    }

    public interface IEsiClient
    {
    }

    public class EsiClient : IEsiClient
    {
    }

    public static class EsiServices
    {
        public static IServiceCollection AddEsi(this IServiceCollection services)
        {
            services.TryAddSingleton<IEsiCache, EsiCache>();
            services.TryAddSingleton<IEsiClient, EsiClient>();

            return services;
        }
    }
}
