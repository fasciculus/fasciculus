using Fasciculus.Eve.Models;
using System.Linq;

namespace Fasciculus.Eve.Operations
{
    public static class ConvertUniverse
    {
        public static EveUniverse Execute(SdeUniverse sdeUniverse)
        {
            EveRegions regions = ConvertRegions(sdeUniverse);

            return new(regions);
        }

        private static EveRegions ConvertRegions(SdeUniverse sdeUniverse)
        {
            return new(sdeUniverse.Regions.Select(ConvertRegion));
        }

        private static EveRegion ConvertRegion(SdeRegion region)
        {
            EveId id = region.RegionID;
            string name = region.Name;
            EveConstellation[] constellations = region.Constellations.Select(ConvertConstellation).ToArray();

            return new(id, name, constellations);
        }

        private static EveConstellation ConvertConstellation(SdeConstellation constellation)
        {
            EveId id = constellation.ConstellationID;
            string name = constellation.Name;
            EveSolarSystem[] solarSystems = constellation.SolarSystems.Select(ConvertSolarSystem).ToArray();

            return new(id, name, solarSystems);
        }

        private static EveSolarSystem ConvertSolarSystem(SdeSolarSystem solarSystem)
        {
            EveId id = solarSystem.SolarSystemID;
            string name = solarSystem.Name;
            EveStargate[] stargates = solarSystem.Stargates.Select(kvp => ConvertStargate(kvp.Key, kvp.Value)).ToArray();

            return new(id, name, stargates);
        }

        private static EveStargate ConvertStargate(int id, SdeStargate stargate)
        {
            int destinationId = stargate.Destination;

            return new(id, destinationId);
        }
    }
}
