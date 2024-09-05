using Fasciculus.Eve.Models;
using Fasciculus.Reflection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Operations
{
    public static class ReadResources
    {
        public static void Read()
        {
            Action[] actions =
            {
                ReadEveUniverse,
                ReadConstellations,
                ReadNames,
                ReadRegions,
                ReadSolarSystems,
                ReadStargates
            };

            actions.Select(Task.Run).WaitAll();
        }

        private static void ReadEveUniverse()
        {
            EmbeddedResources.Read("EveUniverse", EveUniverse.Read);
        }

        private static void ReadConstellations()
        {
            EmbeddedResources.Read("Sde.Constellations", Constellations.Read);
        }

        private static void ReadNames()
        {
            EmbeddedResources.Read("Sde.Names", Names.Read);
        }

        private static void ReadRegions()
        {
            EmbeddedResources.Read("Sde.Regions", Regions.Read);
        }

        private static void ReadSolarSystems()
        {
            EmbeddedResources.Read("Sde.SolarSystems", SolarSystems.Read);
        }

        private static void ReadStargates()
        {
            EmbeddedResources.Read("Sde.Stargates", Stargates.Read);
        }
    }
}
