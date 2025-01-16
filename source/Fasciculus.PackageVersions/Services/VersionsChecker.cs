using Fasciculus.PackageVersions.Models;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Fasciculus.PackageVersions.Services
{
    public class VersionsChecker
    {
        private readonly VersionsProvider versionsProvider;

        public VersionsChecker(VersionsProvider versionsProvider)
        {
            this.versionsProvider = versionsProvider;
        }

        public void Check(IEnumerable<PackageInfo> packages)
        {
            int updateable = packages.Select(Check).Sum();

            Log($"{updateable} package(s) to update.");
        }

        private int Check(PackageInfo package)
        {
            SortedSet<NuGetVersion> versions = versionsProvider.GetVersions(package.Name, package.Version.IsPrerelease);

            if (versions.Any(v => v > package.Version))
            {
                Log($"{package.Name}: {package.Version} -> {versions.Last()}");
                return 1;
            }
            else
            {
                return 0;
            }
        }

        private static void Log(string message)
        {
            Console.WriteLine(message);
            Debug.WriteLine(message);
        }
    }
}
