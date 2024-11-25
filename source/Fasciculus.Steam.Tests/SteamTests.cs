using Fasciculus.Steam.Models;
using Fasciculus.Steam.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Versioning;

namespace Fasciculus.Steam.Tests
{
    [TestClass]
    public class SteamTests
    {
        private ISteamApplications steamApplications = new SteamApplications();

        [TestMethod]
        [SupportedOSPlatform("windows")]
        public void TestAll()
        {
            SteamApplication[] apps = steamApplications.All;
        }

        [TestMethod]
        [SupportedOSPlatform("windows")]
        public void TestInstalled()
        {
            SteamApplication[] apps = steamApplications.Installed;
        }
    }
}
