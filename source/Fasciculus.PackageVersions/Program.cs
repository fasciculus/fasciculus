using Fasciculus.PackageVersions.Models;
using Fasciculus.PackageVersions.Services;
using System.Collections.Generic;

namespace Fasciculus.PackageVersions
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            using VersionsProvider versionsProvider = new();
            VersionsChecker versionsChecker = new(versionsProvider);
            List<PackageInfo> packages = PackagesProvider.GetPackages();

            versionsChecker.Check(packages);
        }
    }
}
