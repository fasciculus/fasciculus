using System.Collections.Generic;

namespace Fasciculus.Eve.Models
{
    public class SdeSolarSystem
    {
        public int SolarSystemID { get; set; }
        public double Security { get; set; }
        public string SecurityClass { get; set; } = string.Empty;
        public Dictionary<int, SdeStargate> Stargates { get; set; } = [];

        public string Name { get; set; } = string.Empty;

        public void Populate(SdeData data)
        {
            Name = data.Names[SolarSystemID];
        }
    }
}
