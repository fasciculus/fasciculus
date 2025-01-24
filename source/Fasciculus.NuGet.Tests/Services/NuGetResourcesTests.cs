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
            INuGetResources resources = new NuGetResources();

            resources.GetFindPackageByIdResource();
        }
    }
}
