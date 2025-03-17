using NuGet.Versioning;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.NuGet.Versioning
{
    public class NuGetVersions : IEnumerable<NuGetVersion>
    {
        public static NuGetVersion Zero { get; } = new NuGetVersion(0, 0, 0);

        private readonly SortedSet<NuGetVersion> versions = new(NuGetVersionComparer.Descending);

        public int Count => versions.Count;

        public NuGetVersion Latest => versions.FirstOrDefault() ?? Zero;

        public NuGetVersions(IEnumerable<NuGetVersion> versions)
        {
            foreach (var version in versions)
            {
                this.versions.Add(version);
            }
        }

        public NuGetVersions(params NuGetVersion[] versions)
            : this(versions.AsEnumerable())
        {
        }

        public IEnumerator<NuGetVersion> GetEnumerator()
            => versions.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => versions.GetEnumerator();
    }
}
