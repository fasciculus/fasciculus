using Fasciculus.IO;
using System.Collections.Generic;

namespace Fasciculus.Eve.Models
{
    public class EveConstellation : EveNamedObject
    {
        private readonly EveSolarSystem[] solarSystems;

        public IEnumerable<EveSolarSystem> SolarSystems
            => solarSystems;

        public EveConstellation(EveId id, string name, EveSolarSystem[] solarSystems)
            : base(id, name)
        {
            this.solarSystems = solarSystems;
        }

        public override void Write(Data data)
        {
            base.Write(data);

            data.WriteInt(solarSystems.Length);
            solarSystems.Apply(solarSystem => solarSystem.Write(data));
        }

    }
}
