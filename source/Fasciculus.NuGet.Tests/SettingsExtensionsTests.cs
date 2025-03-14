using Fasciculus.NuGet.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Fasciculus.NuGet.Tests
{
    [TestClass]
    public class SettingsExtensionsTests
    {
        [TestMethod]
        public void TestLocal()
        {
            NuGetSource source = SettingsLoader.Load().GetLocalPackageSource();

            Assert.IsTrue(Directory.Exists(source.Source.Source));
        }

        [TestMethod]
        public void TestRemote()
        {
            NuGetSources packageSources = SettingsLoader.Load().GetRemotePackageSources();

            Assert.AreEqual(1, packageSources.Count);
        }
    }
}
