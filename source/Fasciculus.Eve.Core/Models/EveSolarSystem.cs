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

            stream.WriteDouble(Security);
            stream.WriteArray(stargates, stargate => stargate.Write(stream));
            stream.WriteArray(planets, planet => planet.Write(stream));
            stream.WriteBool(HasIce);
        }

        public static EveSolarSystem Read(Stream stream)
        {
            (EveId id, string name) = BaseRead(stream);
            double security = stream.ReadDouble();
            EveStargate[] stargates = stream.ReadArray(EveStargate.Read);
            EvePlanet[] planets = stream.ReadArray(EvePlanet.Read);
            bool hasIce = stream.ReadBool();

            return new(id, name, security, stargates, planets, hasIce);
        }

        public override string ToString()
        {
            return $"{Name} ({EveSecurity.Format(Security)}) {Constellation.Region.Name}";
        }
    }
}
