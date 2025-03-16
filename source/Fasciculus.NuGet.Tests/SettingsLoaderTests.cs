using Fasciculus.NuGet.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet.Configuration;
using System.Threading.Tasks;

namespace Fasciculus.NuGet.Tests
{
    [TestClass]
    public class SettingsLoaderTests
    {
        [TestMethod]
        public async Task Test()
        {
            ISettings settings = await SettingsLoader.LoadAsync();
            string[] roots = [.. settings.GetConfigRoots()];

            Assert.IsTrue(roots.Length > 0);
        }
    }
}
