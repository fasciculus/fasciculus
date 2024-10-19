using System.Collections.Generic;

namespace Fasciculus.Eve.Models
{
    public class SdeRegions
    {
        private readonly List<SdeRegion> regions = [];

        public SdeRegions(IEnumerable<SdeRegion> regions)
        {
            regions.Apply(region => { this.regions.Add(region); });
        }

        public void Populate(SdeData data)
        {
            regions.Apply(region => { region.Populate(data); });
        }
    }
}
