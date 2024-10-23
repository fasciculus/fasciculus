using Fasciculus.IO;
using System.Collections.Generic;

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

        public void Write(Data data)
        {
            data.WriteArray(objectsByIndex, o => o.Write(data));
        }

        public static EveRegions Read(Data data)
        {
            EveRegion[] regions = data.ReadArray(EveRegion.Read);

            return new(regions);
        }
    }
}
