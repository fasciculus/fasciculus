using System.Collections.Generic;

namespace Fasciculus.Eve.Models
{
    public class EveConstellations : EveNamedObjects<EveConstellation>
    {
        public EveConstellations(IEnumerable<EveConstellation> constellations)
            : base(constellations) { }
    }
}
