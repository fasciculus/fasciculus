using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using System;
using System.Runtime.Versioning;

namespace Fasciculus.Steam.Tests
{
    [TestClass]
    public class SteamTests
    {
        [TestMethod]
        [SupportedOSPlatform("windows")]
        public void Test()
        {
            using RegistryKey? software = Registry.CurrentUser.OpenSubKey("SOFTWARE", false);
            using RegistryKey? valve = software?.OpenSubKey("Valve", false);
            using RegistryKey? steam = valve?.OpenSubKey("Steam", false);
            using RegistryKey? apps = steam?.OpenSubKey("Apps", false);

            string[] ids = apps?.GetSubKeyNames() ?? [];

            foreach (string id in ids)
            {
                using RegistryKey? app = apps?.OpenSubKey(id, false);
                string? name = app?.GetValue("Name")?.ToString();

                if (name is not null)
                {
                    Console.WriteLine(name);
                }
            }
        }
    }
}
