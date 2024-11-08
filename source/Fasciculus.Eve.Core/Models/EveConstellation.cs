using Fasciculus.IO;
using Fasciculus.Validating;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public class EveConstellation : EveNamedObject
    {
        private EveRegion? region;
        private readonly EveSolarSystem[] solarSystems;

        public EveRegion Region
            => Cond.NotNull(region);

        public IEnumerable<EveSolarSystem> SolarSystems
            => solarSystems;

        public EveConstellation(EveId id, string name, EveSolarSystem[] solarSystems)
            : base(id, name)
        {
            this.solarSystems = solarSystems;
        }

        public IEnumerable<EveConstellation> GetNeighbours(EveSecurity security)
        {
            return solarSystems
                .SelectMany(ss => ss.GetNeighbours(security))
                .Select(ss => ss.Constellation)
                .DistinctBy(c => c.Index)
                .Where(c => c.Index != Index);
        }

        internal void Link(EveRegion region, IEveUniverse universe)
        {
            this.region = region;
            solarSystems.Apply(solarSystem => solarSystem.Link(this, universe));
        }

        public override void Write(Data data)
        {
            base.Write(data);

            data.WriteArray(solarSystems, solarSystem => solarSystem.Write(data));
        }

        public static EveConstellation Read(Data data)
        {
            EveId id = EveId.Read(data);
            string name = data.ReadString();
            EveSolarSystem[] solarSystems = data.ReadArray(EveSolarSystem.Read);

            return new(id, name, solarSystems);
        }
    }
}
