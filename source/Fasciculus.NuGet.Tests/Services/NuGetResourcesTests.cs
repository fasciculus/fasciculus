using Fasciculus.NuGet.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.NuGet.Tests.Services
{
    [TestClass]
    public class NuGetResourcesTests
    {
        [TestMethod]
        public void Test()
        {
            NuGetResources resources = new();

            var findPackageById = resources.FindPackageById;
            var packageMetadata = resources.PackageMetadata;

            Assert.IsNotNull(findPackageById);
            Assert.IsNotNull(packageMetadata);
        }
    }
}
