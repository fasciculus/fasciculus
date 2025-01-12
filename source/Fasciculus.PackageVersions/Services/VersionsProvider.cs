using Fasciculus.Threading;
using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Fasciculus.PackageVersions.Services
{
    public class VersionsProvider : IDisposable
    {
        private readonly SourceCacheContext cache;
        private readonly SourceRepository repository;
        private readonly FindPackageByIdResource resource;
        private readonly ILogger logger;
        private readonly CancellationToken cancellationToken;

        public VersionsProvider()
        {
            cache = new();
            repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
            resource = Tasks.Wait(repository.GetResourceAsync<FindPackageByIdResource>());
            logger = NullLogger.Instance;
            cancellationToken = CancellationToken.None;
        }

        public SortedSet<NuGetVersion> GetVersions(string packageName, bool includePrerelease)
        {
            IEnumerable<NuGetVersion> versions = Tasks.Wait(resource.GetAllVersionsAsync(packageName, cache, logger, cancellationToken));

            if (!includePrerelease)
            {
                versions = versions.Where(v => !v.IsPrerelease);
            }

            return new(versions);
        }

        ~VersionsProvider()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool _)
        {
            cache.Dispose();
        }
    }
}
