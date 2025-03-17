using Fasciculus.Threading;
using NuGet.Common;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fasciculus.NuGet.Protocol
{
    public static partial class NuGetSearch
    {
        public static async Task<FindPackageByIdDependencyInfo?> SearchDependenciesAsync(this IEnumerable<FindPackageByIdResource> finders,
            PackageIdentity identity, SourceCacheContext? cacheContext = null, ILogger? logger = null,
            CancellationToken? ctk = null)
        {
            FindPackageByIdDependencyInfo? dependencies = null;

            cacheContext ??= await NuGetSourceCache.Default;
            logger ??= logger.OrNone();

            foreach (var finder in finders)
            {
                dependencies = await finder.GetDependencyInfoAsync(identity.Id, identity.Version, cacheContext, logger, ctk.OrNone());

                if (dependencies is not null)
                {
                    break;
                }
            }

            return dependencies;
        }
    }
}