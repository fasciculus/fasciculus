using Fasciculus.PackageVersions.Models;
using NuGet.Versioning;
using System.Collections.Generic;
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

            Logging.Log($"{updateable} package(s) to update.");
        }

        private int Check(PackageInfo package)
        {
            SortedSet<NuGetVersion> versions = versionsProvider.GetVersions(package.Name, package.Version.IsPrerelease);

            if (versions.Any(v => v > package.Version))
            {
                Logging.Log($"{package.Name}: {package.Version} -> {versions.Last()}");
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
