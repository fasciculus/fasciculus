using Fasciculus.NuGet.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet.Configuration;

namespace Fasciculus.NuGet.Tests
{
    [TestClass]
    public class SettingsExtensionsTests
    {
        [TestMethod]
        public void TestLocal()
        {
            PackageSource? packageSource = SettingsLoader.Load().GetLocalPackageSource();

            Assert.IsNotNull(packageSource);
        }

        [TestMethod]
        public void TestRemote()
        {
            PackageSources packageSources = SettingsLoader.Load().GetRemotePackageSources();

            Assert.AreEqual(1, packageSources.Count);
        }
    }
}
