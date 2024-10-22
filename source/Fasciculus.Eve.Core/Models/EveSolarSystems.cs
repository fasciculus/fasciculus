using System.Collections.Generic;

namespace Fasciculus.Eve.Models
{
    public class EveSolarSystems : EveNamedObjects<EveSolarSystem>
    {
        public EveSolarSystems(IEnumerable<EveSolarSystem> solarSystems)
            : base(solarSystems)
        {
        }
    }
}
