using Fasciculus.IO;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public class EveRegion : EveNamedObject
    {
        private readonly EveConstellation[] constellations;

        public IEnumerable<EveConstellation> Constellations
            => constellations;

        public EveRegion(EveId id, string name, EveConstellation[] constellations)
            : base(id, name)
        {
            this.constellations = constellations;
        }

        public IEnumerable<EveRegion> GetNeighbours(EveSecurity security)
        {
            return constellations
                .SelectMany(c => c.GetNeighbours(security))
                .Select(c => c.Region)
                .DistinctBy(r => r.Index)
                .Where(r => r.Index != Index)
                .OrderBy(r => r.Index);
        }

        internal void Link(IEveUniverse universe)
        {
            constellations.Apply(constellation => constellation.Link(this, universe));
        }

        public override void Write(Stream stream)
        {
            base.Write(stream);

            Data data = stream;

            data.WriteArray(constellations, constellation => constellation.Write(data));
        }

        public static EveRegion Read(Stream stream)
        {
            Data data = stream;

            EveId id = EveId.Read(data);
            string name = data.ReadString();
            EveConstellation[] constellations = data.ReadArray(EveConstellation.Read);

            return new EveRegion(id, name, constellations);
        }
    }
}
