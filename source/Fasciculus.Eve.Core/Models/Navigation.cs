using Fasciculus.Mathematics;
using System.Collections.Generic;
using System.IO;

namespace Fasciculus.Eve.Models
{
    public class EveNavigation
    {
        public class Data
        {
            private readonly SparseShortMatrix[] distances;

            public IReadOnlyList<SparseShortMatrix> Distances => distances;

            public Data(SparseShortMatrix[] distances)
            {
                this.distances = distances;
            }

            public Data(Stream stream)
                : this(stream.ReadArray(s => new SparseShortMatrix(s))) { }

            public void Write(Stream stream)
            {
                stream.WriteArray(distances, x => x.Write(stream));
            }
        }

        private readonly Data data;

        public EveNavigation(Data data)
        {
            this.data = data;
        }

        public EveNavigation(Stream stream)
            : this(new Data(stream)) { }
    }
}
