using NuGet.Versioning;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fasciculus.NuGet.Services
{
    public interface INuGetVersions
    {
        public Task<SortedSet<NuGetVersion>> GetVersionsAsync(string packageName, bool includePrerelease, CancellationToken? ctk = null);

        public SortedSet<NuGetVersion> GetVersions(string packageName, bool includePrerelease, CancellationToken? ctk = null);
    }
}
