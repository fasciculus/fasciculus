using NuGet.Common;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Fasciculus.NuGet
{
    public static class SourceRepositoryExtensions
    {
        public static IPackageSearchMetadata? GetMetadata(this SourceRepository repository, PackageIdentity package, ILogger? logger = null)
        {
            PackageMetadataResource? resource = repository.GetResource<PackageMetadataResource>();

            return resource?.GetMetadataAsync(package, NuGetCache.Default, logger, CancellationToken.None).GetAwaiter().GetResult();
        }

        public static IPackageSearchMetadata? GetMetadata(this IEnumerable<SourceRepository> repositories, PackageIdentity package,
            ILogger? logger = null)
            => repositories.Select(x => x.GetMetadata(package, logger)).FirstOrDefault(x => x != null);

        public static SortedSet<NuGetVersion> GetVersions(this SourceRepository repository, string package, bool includePrerelease,
            ILogger? logger = null)
        {
            FindPackageByIdResource? resource = repository.GetResource<FindPackageByIdResource>();

            IEnumerable<NuGetVersion> versions = resource?.GetAllVersionsAsync(package, NuGetCache.Default, logger,
                CancellationToken.None).GetAwaiter().GetResult() ?? [];

            if (!includePrerelease)
            {
                versions = versions.Where(v => !v.IsPrerelease);
            }

            return [.. versions];
        }
    }
}
