using Fasciculus.NuGet.Protocol;
using Fasciculus.NuGet.Versioning;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using System.Threading.Tasks;

namespace Fasciculus.NuGet.Tests
{
    [TestClass]
    public class SearchDependenciesTests
    {
        [TestMethod]
        public async Task Test()
        {
            NuGetRepository repository = await NuGetRepository.Global;
            NuGetResources resources = repository.Resources;
            FindPackageByIdResource[] finders = await resources.FindPackageById;
            NuGetVersions versions = await NuGetSearch.SearchVersionsAsync(resources, "NuGet.Protocol");
            PackageIdentity identity = new("NuGet.Protocol", versions.Latest);
            FindPackageByIdDependencyInfo? dependencies = await finders.SearchDependenciesAsync(identity);

            Assert.IsNotNull(dependencies);
        }
    }
}
