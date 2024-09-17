using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Versioning;

namespace Fasciculus.Steam.Tests
{
    [TestClass]
    public class SteamTests
    {
        [TestMethod]
        [SupportedOSPlatform("windows")]
        public void TestAll()
        {
            SteamApp[] apps = SteamApps.All;
        }

        [TestMethod]
        [SupportedOSPlatform("windows")]
        public void TestInstalled()
        {
            SteamApp[] apps = SteamApps.Installed;
        }

    }
}
