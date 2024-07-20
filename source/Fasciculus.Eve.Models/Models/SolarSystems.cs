using Fasciculus.IO;
using System.Collections.Generic;

namespace Fasciculus.Eve.Models
{
    public static class SolarSystems
    {
        private static Dictionary<int, SolarSystem> solarSystems = new();

        public static void Add(SolarSystem solarSystem)
        {
            solarSystems[solarSystem.Id] = solarSystem;
        }

        public static void Write(Data data)
        {
            data.WriteInt(solarSystems.Count);

            foreach (SolarSystem solarSystem in solarSystems.Values)
            {
                solarSystem.Write(data);
            }
        }
    }
}
