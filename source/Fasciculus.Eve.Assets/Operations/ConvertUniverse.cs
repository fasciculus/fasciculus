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
            int id = region.RegionID;
            string name = region.Name;

            return new(id, name);
        }
    }
}
