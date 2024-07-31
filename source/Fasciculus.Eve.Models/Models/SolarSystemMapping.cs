using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public class SolarSystemMapping
    {
        private readonly SolarSystem[] solarSystems;
        private readonly Dictionary<int, int> mapping = new();

        public SolarSystemMapping(IEnumerable<SolarSystem> solarSystems)
        {
            this.solarSystems = solarSystems.ToArray();

            for (int i = 0, n = this.solarSystems.Length; i < n; ++i)
            {
                mapping[this.solarSystems[i].Id] = i;
            }
        }

        public SolarSystem this[int index]
        {
            get { return solarSystems[index]; }
        }

        public int this[SolarSystem index]
        {
            get { return mapping[index.Id]; }
        }
    }
}
