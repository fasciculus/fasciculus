using Fasciculus.IO;
using System.Collections.Generic;

namespace Fasciculus.Eve.Models
{
    public static class Constellations
    {
        private static Dictionary<int, Constellation> constellations = new();

        public static Constellation Get(int id)
        {
            return constellations[id];
        }

        public static void Add(Constellation constellation)
        {
            lock (constellations)
            {
                constellations[constellation.Id] = constellation;
            }
        }

        public static void Write(Data data)
        {
            data.WriteInt(constellations.Count);

            foreach (Constellation constellation in constellations.Values)
            {
                constellation.Write(data);
            }
        }
    }
}
