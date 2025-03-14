using Fasciculus.NuGet.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace Fasciculus.NuGet.Tests
{
    [TestClass]
    public class SettingsExtensionsTests
    {
        [TestMethod]
        public async Task TestLocal()
        {
            NuGetSource source = await NuGetSource.Local;

            Assert.IsTrue(Directory.Exists(source.Source.Source));
        }

        [TestMethod]
        public async Task TestRemote()
        {
            NuGetSources packageSources = await NuGetSources.Remotes;

            Assert.AreEqual(1, packageSources.Count);
        }
    }
}
