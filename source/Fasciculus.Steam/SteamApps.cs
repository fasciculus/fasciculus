using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Steam
{
    public static class SteamApps
    {
        public static SteamApp[] All => GetAll();
        public static SteamApp[] Installed => GetInstalled();

        private static SteamApp[] GetAll()
        {
            using RegistryKey? software = Registry.CurrentUser.OpenSubKey("SOFTWARE", false);
            using RegistryKey? valve = software?.OpenSubKey("Valve", false);
            using RegistryKey? steam = valve?.OpenSubKey("Steam", false);
            using RegistryKey? apps = steam?.OpenSubKey("Apps", false);

            if (apps is null) return [];

            string[] idStrings = apps.GetSubKeyNames();
            List<SteamApp> result = new();

            foreach (string idString in idStrings)
            {
                if (!int.TryParse(idString, out int id)) continue;

                using RegistryKey? app = apps.OpenSubKey(idString, false);

                if (app is null) continue;

                string? name = app.GetValue("Name")?.ToString();

                if (name is null) continue;

                bool installed = false;
                string? installedString = app.GetValue("Installed")?.ToString();

                if (installedString is not null)
                {
                    if (uint.TryParse(installedString, out uint installedValue))
                    {
                        installed = installedValue != 0;
                    }
                }

                SteamApp steamApp = new(id, name, installed, null);

                result.Add(steamApp);
            }

            return result.ToArray();
        }

        private static SteamApp[] GetInstalled()
            => GetAll().Where(a => a.Installed).ToArray();
    }
}
