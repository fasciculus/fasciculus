using Fasciculus.Threading;
using NuGet.Common;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using System.Threading;
using System.Threading.Tasks;

namespace Fasciculus.NuGet.Services
{
    public class MetadataProvider : IMetadataProvider
    {
        private readonly PackageMetadataResource resource;
        private readonly SourceCacheContext cache;
        private readonly ILogger logger;

        public MetadataProvider(INuGetResources resources, SourceCacheContext cache, ILogger logger)
        {
            resource = resources.PackageMetadata;

            this.cache = cache;
            this.logger = logger;
        }

        public Task<IPackageSearchMetadata> GetMetadataAsync(PackageIdentity package, CancellationToken? ctk = null)
        {
            return resource.GetMetadataAsync(package, cache, logger, ctk.OrNone());
        }

        public IPackageSearchMetadata GetMetadata(PackageIdentity package, CancellationToken? ctk = null)
            => Tasks.Wait(GetMetadataAsync(package, ctk));
    }
}
