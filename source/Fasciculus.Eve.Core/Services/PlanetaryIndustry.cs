using Fasciculus.Eve.Models;
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

        private readonly IEveResources resources;

        public PlanetaryIndustry(IEveResources resources)
        {
            this.resources = resources;
        }

        public Task StartAsync()
        {
            using Locker locker = Locker.Lock(mutex);

            return Tasks.LongRunning(Start);
        }

        private void Start()
        {
            EvePlanetSchematics schematics = GetSchematics();

            EveType[] p0 = [.. schematics.P0];
            EvePlanetSchematic[] p1 = [.. schematics.P1];
            EvePlanetSchematic[] p2 = [.. schematics.P2];
            EvePlanetSchematic[] p3 = [.. schematics.P3];
            EvePlanetSchematic[] p4 = [.. schematics.P4];
        }

        private EvePlanetSchematics GetSchematics()
        {
            return Tasks.Wait(resources.Data).PlanetSchematics;
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
