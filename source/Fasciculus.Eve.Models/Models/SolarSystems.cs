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

        public static SolarSystem Get(int id)
            => solarSystems[id];

        public static SolarSystem? Get(string name)
            => solarSystems.Values.First(s => s.Name == name);

        public static void Add(SolarSystem solarSystem)
        {
            lock (solarSystems)
            {
                solarSystems[solarSystem.Id] = solarSystem;
            }

        }

        public static void Read(Data data)
        {
            int count = data.ReadInt();

            for (int i = 0; i < count; i++)
            {
                SolarSystem.Read(data);
            }
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
