using Fasciculus.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public static class SolarSystems
    {
        private static readonly Dictionary<int, SolarSystem> solarSystems = new();

        private static readonly Lazy<List<SolarSystem>> all = new(GetAll);
        private static readonly Lazy<List<SolarSystem>> safe = new(GetSafe);

        public static IReadOnlyList<SolarSystem> All => all.Value;
        public static IReadOnlyList<SolarSystem> Safe => safe.Value;

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

        private static List<SolarSystem> GetAll()
            => solarSystems.Values.OrderBy(s => s.Id).ToList();

        private static List<SolarSystem> GetSafe()
            => All.Where(s => s.Safe).ToList();
    }
}
