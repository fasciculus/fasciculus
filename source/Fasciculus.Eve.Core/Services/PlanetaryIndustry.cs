using Fasciculus.Eve.Models;
using Fasciculus.Maui.Services;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
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

        private readonly IExceptions exceptions;
        private readonly IEveResources resources;

        public PlanetaryIndustry(IExceptions exceptions, IEveResources resources)
        {
            this.exceptions = exceptions;
            this.resources = resources;
        }

        public Task StartAsync()
        {
            using Locker locker = Locker.Lock(mutex);

            return Tasks.LongRunning(Start);
        }

        private void Start()
        {
            exceptions.Clear();

            EvePlanetSchematics schematics = GetSchematics();

            EveType[] p0 = [.. schematics.P0];
            EvePlanetSchematic[] p1 = [.. schematics.P1];
            EvePlanetSchematic[] p2 = [.. schematics.P2];
            EvePlanetSchematic[] p3 = [.. schematics.P3];
            EvePlanetSchematic[] p4 = [.. schematics.P4];

            Tasks.Sleep(2000);
            exceptions.Add(new Exception("Not (yet) implemented"));
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
