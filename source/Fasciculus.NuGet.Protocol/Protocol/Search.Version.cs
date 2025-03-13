using Fasciculus.Threading;
using NuGet.Common;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fasciculus.NuGet.Protocol
{
    public static partial class NuGetSearch
    {
        public static async Task<SortedSet<NuGetVersion>> SearchVersionsAsync(this IEnumerable<FindPackageByIdResource> finders,
            string id, bool includePrerelease = true, SourceCacheContext? cacheContext = null, ILogger? logger = null,
            CancellationToken? cancellationToken = null)
        {
            SortedSet<NuGetVersion> versions = new(VersionComparer.Default);

            cacheContext ??= await NuGetSourceCache.Default;
            logger ??= logger.OrNone();

            foreach (var finder in finders)
            {
                IEnumerable<NuGetVersion> candidates = await finder.GetAllVersionsAsync(id, cacheContext, logger, cancellationToken.OrNone());

                foreach (var candidate in candidates)
                {
                    bool included = includePrerelease || !candidate.IsPrerelease;

                    if (included)
                    {
                        versions.Add(candidate);
                    }
                }
            }

            return versions;
        }

        public static async Task<SortedSet<NuGetVersion>> SearchVersionsAsync(this NuGetResources resources, string id,
            bool includePrerelease = true, SourceCacheContext? cacheContext = null, ILogger? logger = null,
            CancellationToken? cancellationToken = null)
            => await SearchVersionsAsync(await resources.FindPackageById, id, includePrerelease, cacheContext, logger, cancellationToken);
    }
}