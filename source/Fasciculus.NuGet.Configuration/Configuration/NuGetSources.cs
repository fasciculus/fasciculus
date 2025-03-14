using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.NuGet.Configuration
{
    public class NuGetSources : IEnumerable<NuGetSource>
    {
        private readonly NuGetSource[] sources;

        public int Count
            => sources.Length;

        public NuGetSources(IEnumerable<NuGetSource> sources)
        {
            this.sources = [.. sources];
        }

        public IEnumerator<NuGetSource> GetEnumerator()
            => sources.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => sources.AsEnumerable().GetEnumerator();
    }
}
