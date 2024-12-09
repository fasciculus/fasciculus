using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Services
{
    public interface IPlanetaryIndustry
    {
        public Task StartAsync();
    }

    public class PlanetaryIndustry : IPlanetaryIndustry
    {
        private readonly TaskSafeMutex mutex = new();

        public Task StartAsync()
        {
            using Locker locker = Locker.Lock(mutex);

            return Tasks.LongRunning(Start);
        }

        private void Start()
        {

        }
    }

    public static class PlanetaryIndustryServices
    {
        public static IServiceCollection AddPlanetaryIndustry(this IServiceCollection services)
        {
            services.TryAddSingleton<IPlanetaryIndustry, PlanetaryIndustry>();

            return services;
        }
    }
}
