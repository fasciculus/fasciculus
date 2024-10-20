using Fasciculus.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public class EveRegions
    {
        private readonly EveRegion[] regionsByIndex;
        private readonly Dictionary<int, EveRegion> regionsById;

        public EveRegions(IEnumerable<EveRegion> regions)
        {
            regionsByIndex = CreateRegions(regions);
            regionsById = regionsByIndex.ToDictionary(r => r.Id);
        }

        private static EveRegion[] CreateRegions(IEnumerable<EveRegion> regions)
        {
            EveRegion[] result = [.. regions.OrderBy(r => r.Id)];

            for (int i = 0, n = result.Length; i < n; ++i)
            {
                result[i].Index = i;
            }

            return result;
        }

        public EveRegion this[int index]
        {
            get
            {
                if (index >= 0 && index < regionsByIndex.Length)
                {
                    return regionsByIndex[index];
                }

                if (regionsById.TryGetValue(index, out EveRegion? region))
                {
                    return region;
                }

                throw new IndexOutOfRangeException();
            }
        }

        public void Write(Data data)
        {
            data.WriteInt(regionsByIndex.Length);
            regionsByIndex.Apply(r => r.Write(data));
        }
    }
}
