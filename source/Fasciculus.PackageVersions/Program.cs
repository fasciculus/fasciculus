using Fasciculus.NuGet.Logging;
using Fasciculus.NuGet.Services;
using NuGet.Common;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.PackageVersions
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            DirectoryPackages packagesService = new();
            using SourceCacheContext cache = new();
            NuGetResources resources = new();
            ConsoleLogger logger = new(LogLevel.Warning);
            NuGetVersions versionsService = new(resources, cache, logger);

            PackageIdentity[] packages = packagesService.GetPackages();

            foreach (PackageIdentity package in packages)
            {
                bool includePrerelease = package.Version.IsPrerelease;
                SortedSet<NuGetVersion> versions = versionsService.GetVersions(package.Id, includePrerelease);

                if (versions.Any(v => v > package.Version))
                {
                    string message = $"{package.Id}: {package.Version} -> {versions.Last()}";

                    logger.LogWarning(message);
                }
            }
        }
    }
}
