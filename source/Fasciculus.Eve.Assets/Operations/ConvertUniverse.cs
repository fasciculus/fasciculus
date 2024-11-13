using Fasciculus.Eve.Models;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Eve.Operations
{
    public static class ConvertUniverse
    {
        private static readonly SortedSet<string> IceSystemNames =
            [
            "Ahtulaima", "Fuskunen", "Gekutami", "Hentogaira", "Hurtoken", "Mitsolen", "Obe", "Osmon", "Outuni", "Silen", "Sirseshin", "Vattuolen", "Wuos",
            ];

        public static EveUniverse Execute(SdeUniverse sdeUniverse)
        {
            EveRegions regions = ConvertRegions(sdeUniverse);

            return new(regions);
        }

        private static EveRegions ConvertRegions(SdeUniverse sdeUniverse)
        {
            return new(sdeUniverse.Regions.Select(ConvertRegion).ToArray());
        }

        private static EveRegion ConvertRegion(SdeRegion region)
        {
            EveId id = new(region.RegionID);
            string name = region.Name;
            EveConstellation[] constellations = region.Constellations.Select(ConvertConstellation).ToArray();

            return new(id, name, constellations);
        }

        private static EveConstellation ConvertConstellation(SdeConstellation constellation)
        {
            EveId id = new(constellation.ConstellationID);
            string name = constellation.Name;
            EveSolarSystem[] solarSystems = constellation.SolarSystems.Select(ConvertSolarSystem).ToArray();

            return new(id, name, solarSystems);
        }

        private static EveSolarSystem ConvertSolarSystem(SdeSolarSystem solarSystem)
        {
            EveId id = new(solarSystem.SolarSystemID);
            string name = solarSystem.Name;
            double security = solarSystem.Security;
            EveStargate[] stargates = solarSystem.Stargates.Select(kvp => ConvertStargate(kvp.Key, kvp.Value)).ToArray();
            EvePlanets planets = new(solarSystem.Planets.Select(kvp => ConvertPlanet(kvp.Key, kvp.Value)).ToArray());
            bool hasIce = IceSystemNames.Contains(name);

            return new(id, name, security, stargates, planets, hasIce);
        }

        private static EveStargate ConvertStargate(int rawId, SdeStargate stargate)
        {
            EveId id = new(rawId);
            EveId destinationId = new(stargate.Destination);

            return new(id, destinationId);
        }

        private static EvePlanet ConvertPlanet(int rawId, SdePlanet planet)
        {
            EveId id = new(rawId);
            EveCelestialIndex celestialIndex = new(planet.CelestialIndex);
            Queue<EveCelestialIndex> celestialIndices = new(Enumerable.Range(1, planet.Moons.Count).Select(EveCelestialIndex.Create));
            EveMoons moons = new(planet.Moons.Select(kvp => ConvertMoon(kvp.Key, celestialIndices, kvp.Value)).ToArray());

            return new(id, celestialIndex, moons);
        }

        private static EveMoon ConvertMoon(int rawId, Queue<EveCelestialIndex> celestialIndices, SdeMoon moon)
        {
            EveId id = new(rawId);
            EveCelestialIndex celestialIndex = celestialIndices.Dequeue();
            EveNpcStation[] npcStations = moon.NpcStations.Select(kvp => ConvertNpcStation(kvp.Key, kvp.Value)).ToArray();

            return new(id, celestialIndex, npcStations);
        }

        private static EveNpcStation ConvertNpcStation(int rawId, SdeMoonStation moonStation)
        {
            EveId id = new(rawId);
            EveId operationId = new(moonStation.OperationID);
            EveId typeId = new(moonStation.TypeID);

            return new(id, operationId, typeId);
        }
    }
}
