using Fasciculus.Collections;
using Fasciculus.Threading.Synchronization;
using NuGet.Common;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System.Collections.Generic;

namespace Fasciculus.NuGet.Services
{
    public class DependencyProvider : IDependencyProvider
    {
        private readonly IMetadataProvider metadataProvider;
        private readonly IIgnoredPackages ignoredPackages;
        private readonly ILogger logger;

        private readonly TaskSafeMutex mutex = new();

        private readonly Dictionary<PackageIdentity, IPackageSearchMetadata> visited = [];

        public DependencyProvider(IMetadataProvider metadataProvider, IIgnoredPackages ignoredPackages, ILogger logger)
        {
            this.metadataProvider = metadataProvider;
            this.ignoredPackages = ignoredPackages;
            this.logger = logger;
        }

        public IPackageSearchMetadata[] GetDependencies(IEnumerable<PackageIdentity> packages)
        {
            using Locker locker = Locker.Lock(mutex);

            visited.Clear();

            Visit(packages);

            return [.. visited.Values];
        }

        private void Visit(IEnumerable<PackageIdentity> packages)
            => packages.Apply(Visit);

        private void Visit(PackageIdentity package)
        {
            if (visited.ContainsKey(package) || ignoredPackages.IsIgnored(package.Id))
            {
                return;
            }

            IPackageSearchMetadata metadata = metadataProvider.GetMetadata(package);

            visited[package] = metadata;

            Visit(metadata.DependencySets);
        }

        private void Visit(IEnumerable<PackageDependencyGroup> groups)
            => groups.Apply(Visit);

        private void Visit(PackageDependencyGroup group)
            => Visit(group.Packages);

        private void Visit(IEnumerable<PackageDependency> dependencies)
            => dependencies.Apply(Visit);

        private void Visit(PackageDependency dependency)
        {
            string id = dependency.Id;

            if (ignoredPackages.IsIgnored(id))
            {
                return;
            }

            VersionRange versionRange = dependency.VersionRange;

            if (versionRange.HasLowerBound)
            {
                NuGetVersion version = versionRange.MinVersion;
                PackageIdentity package = new(id, version);

                Visit(package);
            }
            else
            {
                string message = $"{id} {versionRange} has no lower bound";

                logger.LogWarning(message);
            }
        }
    }
}
