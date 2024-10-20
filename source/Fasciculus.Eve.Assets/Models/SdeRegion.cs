using System.Collections.Generic;

namespace Fasciculus.Eve.Models
{
    public class SdeRegion
    {
        public int RegionID { get; set; }
        public SdeConstellation[] Constellations { get; set; } = [];

        public string Name { get; set; } = string.Empty;

        public void Populate(SdeData data)
        {
            Name = data.Names[RegionID];

            Constellations.Apply(c => c.Populate(data));
        }
    }
}
