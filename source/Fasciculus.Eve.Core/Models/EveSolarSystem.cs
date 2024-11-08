using Fasciculus.IO;
using Fasciculus.Validating;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public class EveSolarSystem : EveNamedObject
    {
        public double Security { get; }

        private EveConstellation? constellation;
        private readonly EveStargate[] stargates;

        public EveConstellation Constellation
            => Cond.NotNull(constellation);

        public IEnumerable<EveStargate> Stargates
            => stargates;

        public EveSolarSystem(EveId id, string name, double security, EveStargate[] stargates)
            : base(id, name)
        {
            Security = security;
            this.stargates = stargates;
        }

        public IEnumerable<EveSolarSystem> GetNeighbours(EveSecurity security)
        {
            return stargates.Select(sg => sg.Destination.SolarSystem).Where(security.Filter).OrderBy(ss => ss.Index);
        }

        public void Link(EveConstellation constellation, IEveUniverse universe)
        {
            this.constellation = constellation;
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
            return $"{Name} ({EveSecurity.Format(Security)}) {Constellation.Region.Name}";
        }
    }
}
