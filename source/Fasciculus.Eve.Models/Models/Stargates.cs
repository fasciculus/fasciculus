using Fasciculus.IO;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public static class Stargates
    {
        private static readonly Dictionary<int, Stargate> stargates = new();

        public static Stargate Get(int id)
            => stargates[id];

        public static Stargate[] Get(SolarSystem solarSystem)
            => stargates.Values.Where(sg => sg.SolarSystem.Id == solarSystem.Id).ToArray();

        public static void Add(Stargate stargate)
        {
            lock (stargates)
            {
                stargates[stargate.Id] = stargate;
            }
        }

        public static void Read(Data data)
        {
            int count = data.ReadInt();

            for (int i = 0; i < count; i++)
            {
                Stargate.Read(data);
            }
        }

        public static void Write(Data data)
        {
            data.WriteInt(stargates.Count);

            foreach (Stargate stargate in stargates.Values)
            {
                stargate.Write(data);
            }
        }
    }
}