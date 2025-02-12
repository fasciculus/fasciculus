using Fasciculus.IO;
using Fasciculus.Mathematics.LinearAlgebra;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public class EveNavigation
    {
        public class Data
        {
            private readonly SparseShortMatrix[] distances;

            public IReadOnlyList<SparseShortMatrix> Distances => distances;

            public Data(IEnumerable<SparseShortMatrix> distances)
            {
                this.distances = distances.ToArray();
            }

            public Data(BinaryRW bin)
                : this(bin.ReadArray(() => new SparseShortMatrix(bin))) { }

            public void Write(BinaryRW bin)
            {
                bin.WriteArray(distances, x => x.Write(bin));
            }
        }

        private readonly Data data;
        private readonly EveSolarSystems solarSystems;

        public EveNavigation(Data data, EveSolarSystems solarSystems)
        {
            this.data = data;
            this.solarSystems = solarSystems;
        }

        public EveNavigation(BinaryRW bin, EveSolarSystems solarSystems)
            : this(new Data(bin), solarSystems) { }

        public IEnumerable<EveSolarSystem> AtDistance(EveSolarSystem origin, int distance, EveSecurity.Level security)
        {
            if (distance < 1)
            {
                return [];
            }

            SparseShortVector distances = data.Distances[(int)security][origin.Id];
            IEnumerable<uint> ids = distances.Indices.Where(i => distances[i] == distance);

            return ids.Select(id => solarSystems[id]);
        }

        public IEnumerable<EveSolarSystem> InRange(EveSolarSystem origin, int distance, EveSecurity.Level security)
        {
            if (distance < 1)
            {
                return [];
            }

            return Enumerable.Range(1, distance).SelectMany(d => AtDistance(origin, d, security));
        }
    }
}
