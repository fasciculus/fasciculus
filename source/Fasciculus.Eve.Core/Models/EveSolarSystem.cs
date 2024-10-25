using Fasciculus.IO;
using System.Collections.Generic;

namespace Fasciculus.Eve.Models
{
    public class EveSolarSystem : EveNamedObject
    {
        public double Security { get; }

        private readonly EveStargate[] stargates;

        public IEnumerable<EveStargate> Stargates
            => stargates;

        public EveSolarSystem(EveId id, string name, double security, EveStargate[] stargates)
            : base(id, name)
        {
            Security = security;
            this.stargates = stargates;
        }

        public void Link(IEveUniverse universe)
        {
            stargates.Apply(stargate => stargate.Link(this, universe));
        }

        public override void Write(Data data)
        {
            base.Write(data);

            data.WriteDouble(Security);
            data.WriteArray(stargates, stargate => stargate.Write(data));
        }

        public static EveSolarSystem Read(Data data)
        {
            EveId id = EveId.Read(data);
            string name = data.ReadString();
            double security = data.ReadDouble();
            EveStargate[] stargates = data.ReadArray(EveStargate.Read);

            return new(id, name, security, stargates);
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
