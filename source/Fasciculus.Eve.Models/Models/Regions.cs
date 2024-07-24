using Fasciculus.IO;
using System.Collections.Generic;

namespace Fasciculus.Eve.Models
{
    public static class Regions
    {
        private static readonly Dictionary<int, Region> regions = new();

        public static void Add(Region region)
        {
            lock (regions)
            {
                regions[region.Id] = region;
            }
        }

        public static Region Get(int id)
        {
            return regions[id];
        }

        public static void Write(Data data)
        {
            data.WriteInt(regions.Count);

            foreach (Region region in regions.Values)
            {
                region.Write(data);
            }
        }
    }
}
