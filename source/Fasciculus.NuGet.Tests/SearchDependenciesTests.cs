using Fasciculus.NuGet.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System.Collections.Generic;
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
            SortedSet<NuGetVersion> versions = await NuGetSearch.SearchVersionsAsync(resources, "NuGet.Protocol");
            PackageIdentity identity = new("NuGet.Protocol", versions.Max);
            FindPackageByIdDependencyInfo? dependencies = await finders.SearchDependenciesAsync(identity);

            Assert.IsNotNull(dependencies);
        }
    }
}
