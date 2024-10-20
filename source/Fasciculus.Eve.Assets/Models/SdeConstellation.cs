using System.Collections.Generic;

namespace Fasciculus.Eve.Models
{
    public class SdeConstellation
    {
        public int ConstellationID { get; set; }
        public SdeSolarSystem[] SolarSystems { get; set; } = [];

        public string Name { get; set; } = string.Empty;

        public void Populate(SdeData data)
        {
            Name = data.Names[ConstellationID];

            SolarSystems.Apply(s => { s.Populate(data); });
        }
    }
}
