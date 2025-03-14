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
        public async Task TestGlobal()
        {
            NuGetSource source = await NuGetSource.Global;

            Assert.IsTrue(Directory.Exists(source.Source.Source));
        }

        [TestMethod]
        public async Task TestDefault()
        {
            NuGetSources packageSources = await NuGetSources.Default;

            Assert.AreEqual(1, packageSources.Count);
        }
    }
}
