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

            if (result is null)
            {
                var results = Tasks.Wait(parseUniverse.Regions, parseData.Names);
                SdeRegion[] sdeRegions = results.Item1;
                Dictionary<long, string> names = results.Item2;

                progress.ConvertUniverse.Report(PendingToDone.Working);
                result = ConvertRegions(sdeRegions, names);
                progress.ConvertUniverse.Report(PendingToDone.Done);

                await Task.Yield();
            }

            return result;
        }

        private static EveUniverse ConvertRegions(SdeRegion[] sdeRegions, Dictionary<long, string> names)
        {
            EveRegion.Data[] regions = [.. sdeRegions.Select(r => ConvertRegion(r, names)).OrderBy(r => r.Id)];

            return new(new EveUniverse.Data(regions));
        }

        private static EveRegion.Data ConvertRegion(SdeRegion sdeRegion, Dictionary<long, string> names)
        {
            int id = sdeRegion.RegionID;
            string name = names[id];

            EveConstellation.Data[] constellations = [.. sdeRegion.Constellations
                .Select(c => ConvertConstellation(c, names))
                .OrderBy(c => c.Id)];

            return new(id, name, constellations);
        }

        private static EveConstellation.Data ConvertConstellation(SdeConstellation sdeConstellation, Dictionary<long, string> names)
        {
            int id = sdeConstellation.ConstellationID;
            string name = names[id];

            EveSolarSystem.Data[] solarSystems = [.. sdeConstellation.SolarSystems
                .Select(s => ConvertSolarSystem(s, names))
                .OrderBy(s => s.Id)];

            return new(id, name, solarSystems);
        }

        private static EveSolarSystem.Data ConvertSolarSystem(SdeSolarSystem sdeSolarSystem, Dictionary<long, string> names)
        {
            int id = sdeSolarSystem.SolarSystemID;
            string name = names[id];
            double security = sdeSolarSystem.Security;

            EvePlanet.Data[] planets = [.. sdeSolarSystem.Planets
                .Select(ConvertPlanet)
                .OrderBy(p => p.Id)];

            EveStargate.Data[] stargates = [.. sdeSolarSystem.Stargates
                .Select(ConvertStargate)
                .OrderBy(sg => sg.Id)];

            return new(id, name, security, planets, stargates);
        }

        private static EvePlanet.Data ConvertPlanet(KeyValuePair<int, SdePlanet> kvp)
        {
            SdePlanet sdePlanet = kvp.Value;

            int id = kvp.Key;
            int celestialIndex = sdePlanet.CelestialIndex;

            EveMoon.Data[] moons = [.. sdePlanet.Moons
                .Select(ConvertMoon)
                .OrderBy(m => m.Id)];

            return new(id, celestialIndex, moons);
        }

        private static EveMoon.Data ConvertMoon(KeyValuePair<int, SdeMoon> kvp)
        {
            SdeMoon sdeMoon = kvp.Value;

            int id = kvp.Key;
            int celestialIndex = sdeMoon.CelestialIndex;

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
