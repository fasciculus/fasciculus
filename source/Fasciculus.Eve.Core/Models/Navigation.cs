using Fasciculus.IO.Binary;
using Fasciculus.Mathematics.Matrices;
using Fasciculus.Mathematics.Vectors;
using System.Collections.Generic;
using System.IO;
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

            public Data(Stream stream)
                : this(stream.ReadArray(s => new SparseShortMatrix(s))) { }

            public void Write(Stream stream)
            {
                stream.WriteArray(distances, (s, x) => x.Write(s));
            }
        }

        private readonly Data data;
        private readonly EveSolarSystems solarSystems;

        public EveNavigation(Data data, EveSolarSystems solarSystems)
        {
            this.data = data;
            this.solarSystems = solarSystems;
        }

        public EveNavigation(Stream stream, EveSolarSystems solarSystems)
            : this(new Data(stream), solarSystems) { }

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
