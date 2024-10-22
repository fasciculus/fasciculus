using Fasciculus.IO;
using System.Collections.Generic;

namespace Fasciculus.Eve.Models
{
    public class EveSolarSystem : EveNamedObject
    {
        private readonly EveStargate[] stargates;

        public IEnumerable<EveStargate> Stargates
            => stargates;

        public EveSolarSystem(EveId id, string name, EveStargate[] stargates)
            : base(id, name)
        {
            this.stargates = stargates;
        }

        public override void Write(Data data)
        {
            base.Write(data);

            data.WriteInt(stargates.Length);
            stargates.Apply(stargate => stargate.Write(data));
        }
    }
}
