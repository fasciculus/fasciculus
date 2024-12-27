using Fasciculus.Eve.Assets.Models;
using Fasciculus.Eve.Models;
using Fasciculus.Threading;
using Fasciculus.Threading.Synchronization;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IConvertUniverse
    {
        public Task<EveUniverse.Data> Universe { get; }
    }

    public class ConvertUniverse : IConvertUniverse
    {
        private readonly IParseData parseData;
        private readonly IParseUniverse parseUniverse;
        private readonly IAssetsProgress progress;

        private EveUniverse.Data? universe = null;
        private readonly TaskSafeMutex universeMutex = new();

        public Task<EveUniverse.Data> Universe => GetUniverseAsync();

        public ConvertUniverse(IParseData parseData, IParseUniverse parseUniverse, IAssetsProgress progress)
        {
            this.parseData = parseData;
            this.parseUniverse = parseUniverse;
            this.progress = progress;
        }

        private Task<EveUniverse.Data> GetUniverseAsync()
        {
            return Tasks.LongRunning(GetUniverse);
        }

        private EveUniverse.Data GetUniverse()
        {
            using Locker locker = Locker.Lock(universeMutex);

            if (universe is null)
            {
                var results = Tasks.Wait(parseUniverse.Regions, parseData.Names);
                SdeRegion[] sdeRegions = results.Item1;
                Dictionary<uint, string> names = results.Item2;

                progress.ConvertUniverse.Begin(2);
                progress.ConvertUniverse.Report(1);
                universe = ConvertRegions(sdeRegions, names);
                progress.ConvertUniverse.End();
            }

            return universe;
        }

        private static EveUniverse.Data ConvertRegions(SdeRegion[] sdeRegions, Dictionary<uint, string> names)
        {
            var regions = sdeRegions.Select(r => ConvertRegion(r, names)).OrderBy(r => r.Id);

            return new EveUniverse.Data(regions);
        }

        private static EveRegion.Data ConvertRegion(SdeRegion sdeRegion, Dictionary<uint, string> names)
        {
            uint id = sdeRegion.RegionID;
            string name = names[id];
            var constellations = sdeRegion.Constellations.Select(c => ConvertConstellation(c, names)).OrderBy(c => c.Id);

            return new(id, name, constellations);
        }

        private static EveConstellation.Data ConvertConstellation(SdeConstellation sdeConstellation, Dictionary<uint, string> names)
        {
            uint id = sdeConstellation.ConstellationID;
            string name = names[id];
            var solarSystems = sdeConstellation.SolarSystems.Select(s => ConvertSolarSystem(s, names)).OrderBy(s => s.Id);

            return new(id, name, solarSystems);
        }

        private static EveSolarSystem.Data ConvertSolarSystem(SdeSolarSystem sdeSolarSystem, Dictionary<uint, string> names)
        {
            uint id = sdeSolarSystem.SolarSystemID;
            string name = names[id];
            double security = sdeSolarSystem.Security;
            var planets = sdeSolarSystem.Planets.Select(ConvertPlanet).OrderBy(p => p.Id);
            var stargates = sdeSolarSystem.Stargates.Select(ConvertStargate).OrderBy(sg => sg.Id);

            return new(id, name, security, planets, stargates);
        }

        private static EvePlanet.Data ConvertPlanet(KeyValuePair<int, SdePlanet> kvp)
        {
            SdePlanet sdePlanet = kvp.Value;

            int id = kvp.Key;
            int celestialIndex = sdePlanet.CelestialIndex;
            var moons = sdePlanet.Moons.Select(ConvertMoon).OrderBy(m => m.Id);

            return new(id, celestialIndex, moons);
        }

        private static EveMoon.Data ConvertMoon(KeyValuePair<int, SdeMoon> kvp)
        {
            SdeMoon sdeMoon = kvp.Value;

            int id = kvp.Key;
            int celestialIndex = sdeMoon.CelestialIndex;
            var stations = sdeMoon.NpcStations.Select(ConvertMoonStation).OrderBy(x => x.Id);

            return new(id, celestialIndex, stations);
        }

        private static EveStation.Data ConvertMoonStation(KeyValuePair<int, SdeMoonStation> kvp)
        {
            SdeMoonStation sdeMoonStation = kvp.Value;

            int id = kvp.Key;
            int operation = sdeMoonStation.OperationID;
            int owner = sdeMoonStation.OwnerID;

            return new(id, operation, owner);
        }

        private static EveStargate.Data ConvertStargate(KeyValuePair<uint, SdeStargate> kvp)
        {
            uint id = kvp.Key;
            uint destination = kvp.Value.Destination;

            return new(id, destination);
        }
    }
}
