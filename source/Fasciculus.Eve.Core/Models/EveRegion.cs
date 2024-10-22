using Fasciculus.IO;
using System.Collections.Generic;

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

        public override void Write(Data data)
        {
            base.Write(data);

            data.WriteInt(constellations.Length);
            constellations.Apply(c => c.Write(data));
        }
    }
}
