using System.Collections.Generic;

namespace Fasciculus.Eve.Models
{
    public class SdeUniverse
    {
        public List<SdeRegion> Regions { get; } = [];

        public SdeUniverse(IEnumerable<SdeRegion> regions)
        {
            regions.Apply(Regions.Add);
        }

        public void Populate(SdeData data)
        {
            Regions.Apply(region => { region.Populate(data); });
        }
    }
}
