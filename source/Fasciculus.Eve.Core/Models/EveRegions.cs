using System.Collections.Generic;

namespace Fasciculus.Eve.Models
{
    public class EveRegions : EveNamedObjects<EveRegion>
    {
        public EveRegions(IEnumerable<EveRegion> regions)
            : base(regions) { }
    }
}
