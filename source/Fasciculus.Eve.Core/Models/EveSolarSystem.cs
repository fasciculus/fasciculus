using Fasciculus.IO;
using Fasciculus.Validating;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public class EveSolarSystem : EveNamedObject
    {
        public double Security { get; }

        private EveConstellation? constellation;
        private readonly EveStargate[] stargates;
        private readonly EvePlanet[] planets;

        public EveConstellation Constellation
            => Cond.NotNull(constellation);

        public IEnumerable<EveStargate> Stargates
            => stargates;

        public IEnumerable<EvePlanet> Planets
            => planets;

        public bool HasIce { get; private set; }

        public EveSolarSystem(EveId id, string name, double security, EveStargate[] stargates, EvePlanet[] planets, bool hasIce)
            : base(id, name)
        {
            Security = security;
            this.stargates = stargates;
            this.planets = planets;
            HasIce = hasIce;
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

        public override void Write(Stream stream)
        {
            base.Write(stream);

            Data data = stream;

            data.WriteDouble(Security);
            data.WriteArray(stargates, stargate => stargate.Write(data));
            data.WriteArray(planets, planet => planet.Write(data));
            data.WriteBool(HasIce);
        }

        public static EveSolarSystem Read(Stream stream)
        {
            Data data = stream;

            EveId id = EveId.Read(data);
            string name = data.ReadString();
            double security = data.ReadDouble();
            EveStargate[] stargates = data.ReadArray(EveStargate.Read);
            EvePlanet[] planets = data.ReadArray(EvePlanet.Read);
            bool hasIce = data.ReadBool();

            return new(id, name, security, stargates, planets, hasIce);
        }

        public override string ToString()
        {
            return $"{Name} ({EveSecurity.Format(Security)}) {Constellation.Region.Name}";
        }
    }
}
