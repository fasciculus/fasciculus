using Fasciculus.Windows;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Steam
{
    public static class SteamApps
    {
        public static SteamApp[] All
            => RegistryInfo.Read(AppsPath).Children.Select(GetApp).NotNull().ToArray();

        public static SteamApp[] Installed
            => All.Where(a => a.IsInstalled).ToArray();

        private static readonly RegistryPath AppsPath
            = new(RegistryHive.CurrentUser, "SOFTWARE", "Valve", "Steam", "Apps");

        private static readonly RegistryPath UninstallPath
            = new(RegistryHive.LocalMachine, "SOFTWARE", "Microsoft", "Windows", "CurrentVersion", "Uninstall");

        private static SteamApp? GetApp(RegistryPath path)
        {
            if (GetAppData(path, out int id, out string name, out bool installed))
            {
                DirectoryInfo? installPath = installed ? GetInstallPath(id) : null;

                return new(id, name, installPath);
            }

            return null;
        }

        private static bool GetAppData(RegistryPath path, out int id, out string name, out bool installed)
        {
            if (int.TryParse(path.Name, out id))
            {
                RegistryValues values = RegistryInfo.Read(path).Values;

                name = values.GetString("Name");

                installed = values.GetUInt("Installed") != 0;
            }
            else
            {
                name = string.Empty;
                installed = false;
            }

            return name.Length > 0;
        }

        private static DirectoryInfo? GetInstallPath(int id)
        {
            string path = RegistryInfo.Read(UninstallPath.Combine($"Steam App {id}")).Values.GetString("InstallLocation");

            return Directory.Exists(path) ? new(path) : null;
        }
    }
}
