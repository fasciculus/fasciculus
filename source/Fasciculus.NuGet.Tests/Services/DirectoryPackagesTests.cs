using Fasciculus.NuGet.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet.Packaging.Core;

namespace Fasciculus.NuGet.Tests.Services
{
    [TestClass]
    public class DirectoryPackagesTests
    {
        [TestMethod]
        public void Test()
        {
            DirectoryPackagesProvider service = new();
            PackageIdentity[] packages = service.GetPackages();

            Assert.AreEqual(50, packages.Length);
        }
    }
}
