using Fasciculus.IO;
using Fasciculus.IO.Searching;
using Fasciculus.NuGet.Services;
using Fasciculus.Site.Licenses.Models;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Site.Licenses.Services
{
    public class LicensesContent
    {
        private readonly IProjectPackagesProvider projectPackagesProvider;
        private readonly IDirectoryPackagesProvider directoryPackagesProvider;
        private readonly IDependencyProvider dependencyProvider;

        private readonly SearchPath searchPath;
        private readonly Dictionary<string, PackageIdentity> directoryPackages;

        private readonly Dictionary<string, LicenseList> licenseLists = [];

        public LicenseList this[string packageName] => licenseLists[packageName];

        public LicensesContent(IProjectPackagesProvider projectPackagesProvider, IDirectoryPackagesProvider directoryPackagesProvider,
            IDependencyProvider dependencyProvider)
        {
            this.projectPackagesProvider = projectPackagesProvider;
            this.directoryPackagesProvider = directoryPackagesProvider;
            this.dependencyProvider = dependencyProvider;

            searchPath = GetSearchPath();
            directoryPackages = GetDirectoryPackages();

            foreach (string packageName in SiteConstants.PackageNames)
            {
                LicenseList licenseList = new(packageName);
                PackageIdentity[] packages = GetProjectPackages(packageName);

                foreach (NuGetFramework framework in SiteConstants.PackageFrameworks[packageName])
                {
                    IPackageSearchMetadata[] dependencies = dependencyProvider.GetDependencies(packages, framework);

                    licenseList.AddOrMergeWith(dependencies);
                }

                licenseLists[packageName] = licenseList;
            }
        }

        public LicenseList[] GetLicenseLists()
        {
            return [.. licenseLists.Values.Select(list => list.Clone()).OrderBy(list => list.Package)];
        }

        private PackageIdentity[] GetProjectPackages(string packageName)
        {
            PackageIdentity[] packages = projectPackagesProvider.GetPackages([packageName], searchPath);

            return [.. packages.Select(p => directoryPackages[p.Id])];
        }

        private Dictionary<string, PackageIdentity> GetDirectoryPackages()
        {
            return directoryPackagesProvider.GetPackages().ToDictionary(p => p.Id);
        }

        private static SearchPath GetSearchPath()
        {
            FileInfo solutionFile = FileSearch.Search("fasciculus.sln", SearchPath.WorkingDirectoryAndParents).First();
            DirectoryInfo sourceDirectory = solutionFile.Directory!.Combine("source");

            return new(sourceDirectory.GetDirectories());
        }
    }
}
