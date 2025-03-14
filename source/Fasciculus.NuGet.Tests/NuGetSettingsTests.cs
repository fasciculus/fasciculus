using Fasciculus.NuGet.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Fasciculus.NuGet.Tests
{
    [TestClass]
    public class NuGetSettingsTests
    {
        [TestMethod]
        public async Task TestDefault()
        {
            NuGetSettings settings = await NuGetSettings.Default;
            string[] roots = [.. settings.Settings.GetConfigRoots()];

            Assert.IsTrue(roots.Length > 0);
        }
    }
}
