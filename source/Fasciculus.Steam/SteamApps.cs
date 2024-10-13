using Fasciculus.Windows;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Steam
{
    public static class SteamApps
    {
        public static SteamApp[] All => GetAll();
        public static SteamApp[] Installed => GetInstalled();

        private static readonly RegistryPath AppsPath = new(RegistryHive.CurrentUser, "SOFTWARE", "Valve", "Steam", "Apps");

        private static SteamApp[] GetAll()
            => RegistryInfo.Read(AppsPath).Children.Select(GetApp).NotNull().ToArray();

        private static SteamApp[] GetInstalled()
            => GetAll().Where(a => a.Installed).ToArray();

        private static SteamApp? GetApp(RegistryPath path)
        {
            if (int.TryParse(path.Name, out var id))
            {
                RegistryValues values = RegistryInfo.Read(path).Values;

                string name = values.GetString("Name");
                bool installed = values.GetUInt("Installed") != 0;

                return new(id, name, installed, null);
            }

            return null;
        }
    }
}
