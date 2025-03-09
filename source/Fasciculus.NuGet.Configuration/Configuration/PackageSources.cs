using NuGet.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.NuGet.Configuration
{
    public class PackageSources : IEnumerable<PackageSource>
    {
        private readonly PackageSource[] packageSources;

        public int Count => packageSources.Length;

        public PackageSource this[int index] => packageSources[index];

        public PackageSources(IEnumerable<PackageSource> packageSources)
        {
            this.packageSources = [.. packageSources];
        }

        public IEnumerator<PackageSource> GetEnumerator()
            => packageSources.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => packageSources.AsEnumerable().GetEnumerator();
    }
}
