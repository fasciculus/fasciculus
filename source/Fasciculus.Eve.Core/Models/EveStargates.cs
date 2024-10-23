using System.Collections.Generic;

namespace Fasciculus.Eve.Models
{
    public class EveStargates : EveObjects<EveStargate>
    {
        public EveStargates(IEnumerable<EveStargate> stargates)
            : base(stargates) { }
    }
}
