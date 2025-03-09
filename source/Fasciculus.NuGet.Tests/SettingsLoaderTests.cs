using Fasciculus.NuGet.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet.Configuration;

namespace Fasciculus.NuGet.Tests
{
    [TestClass]
    public class SettingsLoaderTests
    {
        [TestMethod]
        public void Test()
        {
            ISettings settings = SettingsLoader.Load();
            string[] roots = [.. settings.GetConfigRoots()];

            Assert.IsTrue(roots.Length > 0);
        }
    }
}
