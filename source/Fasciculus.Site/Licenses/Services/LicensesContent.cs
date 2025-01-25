using Fasciculus.NuGet.Services;
using NuGet.Frameworks;

namespace Fasciculus.Site.Licenses.Services
{
    public class LicensesContent
    {
        private readonly IProjectPackagesProvider projectPackagesProvider;
        private readonly IDirectoryPackagesProvider directoryPackagesProvider;
        private readonly IDependencyProvider dependencyProvider;

        public LicensesContent(IProjectPackagesProvider projectPackagesProvider, IDirectoryPackagesProvider directoryPackagesProvider,
            IDependencyProvider dependencyProvider)
        {
            this.projectPackagesProvider = projectPackagesProvider;
            this.directoryPackagesProvider = directoryPackagesProvider;
            this.dependencyProvider = dependencyProvider;

            foreach (string packageName in SiteConstants.PackageNames)
            {
                foreach (NuGetFramework framework in SiteConstants.PackageFrameworks[packageName])
                {
                }
            }
        }
    }
}
