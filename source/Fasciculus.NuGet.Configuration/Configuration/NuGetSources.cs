using Fasciculus.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fasciculus.NuGet.Configuration
{
    public class NuGetSources : IEnumerable<NuGetSource>
    {
        public static AsyncLazy<NuGetSources> Default { get; } = new(GetDefault);

        private static async Task<NuGetSources> GetDefault()
        {
            NuGetSettings settings = await NuGetSettings.Default;

            return settings.GetDefaultSources();
        }

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
