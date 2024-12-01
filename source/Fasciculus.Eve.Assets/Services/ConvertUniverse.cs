using Fasciculus.Eve.Assets.Models;
using Fasciculus.Eve.Models;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IConvertUniverse
    {
        public Task<EveUniverse> ConvertAsync();
    }

    public class ConvertUniverse : IConvertUniverse
    {
        private readonly IParseData parseData;
        private readonly IParseUniverse parseUniverse;
        private readonly IAssetsProgress progress;

        private EveUniverse? result = null;
        private readonly TaskSafeMutex mutex = new();

        public ConvertUniverse(IParseData parseData, IParseUniverse parseUniverse, IAssetsProgress progress)
        {
            this.parseData = parseData;
            this.parseUniverse = parseUniverse;
            this.progress = progress;
        }

        public async Task<EveUniverse> ConvertAsync()
        {
            using Locker locker = Locker.Lock(mutex);

            await Task.Yield();

            if (result is null)
            {
                Task<SdeRegion[]> sdeRegions = parseUniverse.ParseAsync();
                Task<SdeData> sdeData = parseData.ParseAsync();

                Task.WaitAll([sdeRegions, sdeData]);

                progress.ConvertUniverse.Report(PendingToDone.Working);
                result = ConvertRegions(sdeRegions.Result, sdeData.Result);
                progress.ConvertUniverse.Report(PendingToDone.Done);
            }

            return result;
        }

        private static EveUniverse ConvertRegions(SdeRegion[] sdeRegions, SdeData sdeData)
        {
            EveRegion.Data[] regions = sdeRegions
                    .Select(r => ConvertRegion(r, sdeData))
                    .OrderBy(r => r.Id)
                    .ToArray();

            return new(new EveUniverse.Data(regions));
        }

        private static EveRegion.Data ConvertRegion(SdeRegion sdeRegion, SdeData sdeData)
        {
            int id = sdeRegion.RegionID;
            string name = sdeData.Names[id];

            EveConstellation.Data[] constellations = sdeRegion.Constellations
                .Select(c => ConvertConstellation(c, sdeData))
                .OrderBy(c => c.Id)
                .ToArray();

            return new(id, name, constellations);
        }

        private static EveConstellation.Data ConvertConstellation(SdeConstellation sdeConstellation, SdeData sdeData)
        {
            int id = sdeConstellation.ConstellationID;
            string name = sdeData.Names[id];

            EveSolarSystem.Data[] solarSystems = sdeConstellation.SolarSystems
                .Select(s => ConvertSolarSystem(s, sdeData))
                .OrderBy(s => s.Id)
                .ToArray();

            return new(id, name, solarSystems);
        }

        private static EveSolarSystem.Data ConvertSolarSystem(SdeSolarSystem sdeSolarSystem, SdeData sdeData)
        {
            int id = sdeSolarSystem.SolarSystemID;
            string name = sdeData.Names[id];
            double security = sdeSolarSystem.Security;

            EvePlanet.Data[] planets = sdeSolarSystem.Planets
                .Select(ConvertPlanet)
                .OrderBy(p => p.Id)
                .ToArray();

            EveStargate.Data[] stargates = sdeSolarSystem.Stargates
                .Select(ConvertStargate)
                .OrderBy(sg => sg.Id)
                .ToArray();

            return new(id, name, security, planets, stargates);
        }

        private static EvePlanet.Data ConvertPlanet(KeyValuePair<int, SdePlanet> kvp)
        {
            SdePlanet sdePlanet = kvp.Value;

            int id = kvp.Key;
            int celestialIndex = sdePlanet.CelestialIndex;

            return new(id, celestialIndex);
        }

        private static EveStargate.Data ConvertStargate(KeyValuePair<int, SdeStargate> kvp)
        {
            int id = kvp.Key;
            int destination = kvp.Value.Destination;

            return new(id, destination);
        }
    }

    public static class ConvertUniverseServices
    {
        public static IServiceCollection AddConvertUniverse(this IServiceCollection services)
        {
            services.AddAssetsProgress();
            services.AddParseData();
            services.AddParseUniverse();

            services.TryAddSingleton<IConvertUniverse, ConvertUniverse>();

            return services;
        }
    }
}
