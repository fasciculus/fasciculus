using System.Collections.Generic;

namespace Fasciculus.Eve.Models
{
    public static class Regions
    {
        private static Dictionary<int, Region> regions = new();

        public static void Add(Region region)
        {
            regions[region.Id] = region;
        }

        public static Region Get(int id)
        {
            return regions[id];
        }
    }
}
