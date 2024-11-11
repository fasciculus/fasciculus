using Fasciculus.IO;
using System.Collections.Generic;
using System.IO;

namespace Fasciculus.Eve.Models
{
    public class EveRegions : EveNamedObjects<EveRegion>
    {
        public EveRegions(IEnumerable<EveRegion> regions)
            : base(regions) { }

        internal void Link(IEveUniverse universe)
        {
            this.Apply(region => region.Link(universe));
        }

        public void Write(Stream stream)
        {
            Data data = stream;

            data.WriteArray(objectsByIndex, o => o.Write(data));
        }

        public static EveRegions Read(Stream stream)
        {
            Data data = stream;

            EveRegion[] regions = data.ReadArray(EveRegion.Read);

            return new(regions);
        }
    }
}
