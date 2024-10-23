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

        public void Link(IEveUniverse universe)
        {
            stargates.Apply(stargate => stargate.Link(this, universe));
        }

        public override void Write(Data data)
        {
            base.Write(data);

            data.WriteArray(stargates, stargate => stargate.Write(data));
        }

        public static EveSolarSystem Read(Data data)
        {
            EveId id = EveId.Read(data);
            string name = data.ReadString();
            EveStargate[] stargates = data.ReadArray(EveStargate.Read);

            return new(id, name, stargates);
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
