using Fasciculus.Collections;
using Fasciculus.NuGet;
using Fasciculus.NuGet.Frameworks;
using Fasciculus.NuGet.Services;
using NuGet.Common;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Dependencies
{
    public class Program : IDisposable
    {
        private readonly DirectoryPackagesProvider directoryPackages = new();
        private readonly SourceCacheContext cache = new();
        private readonly NuGetResources resources = new();
        private readonly IgnoredPackages ignoredPackages = new();
        private readonly ConsoleLogger logger = new(LogLevel.Warning);

        private readonly VersionsProvider versionsProvider;
        private readonly MetadataProvider metadataProvider;
        private readonly DependencyProvider dependencyProvider;

        public Program()
        {
            versionsProvider = new(resources, cache, logger);
            metadataProvider = new(resources, cache, logger);
            dependencyProvider = new(metadataProvider, ignoredPackages, logger);
        }

        ~Program()
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

        private void Run()
        {
            PackageIdentity[] packages = directoryPackages.GetPackages();

            logger.LogWarning("Checking for upgrades.");
            CheckForUpgrades(packages);

            packages = FilterPackages(packages);

            logger.LogWarning("Checking licenses.");
            CheckLicenses(packages);
        }

        private void CheckForUpgrades(PackageIdentity[] packages)
        {
            Tuple<PackageIdentity, NuGetVersion>[] upgradables = [.. packages.Select(GetUpgrade).NotNull()];

            if (upgradables.Length > 0)
            {
                logger.LogWarning($"--- {upgradables.Length} upgradables ---");
                upgradables.Apply(x => { logger.LogWarning($"{x.Item1.Id} {x.Item1.Version} -> {x.Item2}"); });
            }
        }

        private Tuple<PackageIdentity, NuGetVersion>? GetUpgrade(PackageIdentity package)
        {
            logger.LogWarning($"- Checking {package.Id}");

            bool includePrerelease = package.Version.IsPrerelease;
            SortedSet<NuGetVersion> versions = versionsProvider.GetVersions(package.Id, includePrerelease);

            if (versions.Any(v => v > package.Version))
            {
                return Tuple.Create(package, versions.Last());
            }

            return null;
        }

        private void CheckLicenses(PackageIdentity[] packages)
        {
            NuGetFramework targetFramework = MoreFrameworks.Net90;
            IPackageSearchMetadata[] dependencies = dependencyProvider.GetDependencies(packages, targetFramework);
            SortedSet<string> dependencyIds = new(dependencies.Select(x => x.Identity.Id));

            dependencyIds.Apply(logger.LogInformation);

            IPackageSearchMetadata[] nonMIT = [.. dependencies
                .Where(x => x.LicenseMetadata is not null)
                .Where(x => x.LicenseMetadata.License != "MIT")
                .Where(x => x.LicenseMetadata.License != "MIT AND Apache-2.0")];

            if (nonMIT.Length > 0)
            {
                logger.LogWarning($"--- {nonMIT.Length} non-MIT dependencies ---");
                nonMIT.Apply(x => { logger.LogWarning($"{x.Identity.Id} -> {x.LicenseMetadata.License}"); });
            }
        }

        private static PackageIdentity[] FilterPackages(PackageIdentity[] packages)
        {
            SortedSet<string> included = ["System.Memory", "CommunityToolkit.Mvvm", "Microsoft.Extensions.DependencyInjection"];

            return [.. packages.Where(x => included.Contains(x.Id))];
        }

        public static void Main(string[] args)
        {
            using Program program = new();

            program.Run();
        }
    }
}
