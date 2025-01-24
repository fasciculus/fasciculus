using Fasciculus.Threading;
using NuGet.Common;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fasciculus.NuGet.Services
{
    public class NuGetVersions : INuGetVersions
    {
        private readonly FindPackageByIdResource resource;
        private readonly SourceCacheContext cache;
        private readonly ILogger logger;

        public NuGetVersions(INuGetResources resources, SourceCacheContext cache, ILogger logger)
        {
            resource = resources.GetFindPackageByIdResource();

            this.cache = cache;
            this.logger = logger;
        }

        public async Task<SortedSet<NuGetVersion>> GetVersionsAsync(string packageName, bool includePrerelease, CancellationToken? ctk = null)
        {
            IEnumerable<NuGetVersion> versions = await resource.GetAllVersionsAsync(packageName, cache, logger, ctk.OrNone());

            if (!includePrerelease)
            {
                versions = versions.Where(v => !v.IsPrerelease);
            }

            return new(versions);
        }

        public SortedSet<NuGetVersion> GetVersions(string packageName, bool includePrerelease, CancellationToken? ctk = null)
            => Tasks.Wait(GetVersionsAsync(packageName, includePrerelease, ctk));
    }
}
