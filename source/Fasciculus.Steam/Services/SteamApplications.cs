using Fasciculus.Steam.Models;
using Fasciculus.Windows.WindowsRegistry;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Steam.Services
{
    public interface ISteamApplications
    {
        public SteamApplication[] All { get; }
        public SteamApplication[] Installed { get; }

        public SteamApplication? this[int id] { get; }
        public SteamApplication? this[string name] { get; }
    }

    public class SteamApplications : ISteamApplications
    {
        private static readonly RegistryPath AppsPath
            = new(RegistryHive.CurrentUser, "SOFTWARE", "Valve", "Steam", "Apps");

        private static readonly RegistryPath UninstallPath
            = new(RegistryHive.LocalMachine, "SOFTWARE", "Microsoft", "Windows", "CurrentVersion", "Uninstall");

        public SteamApplication[] All
            => RegistryInfo.Read(AppsPath).Children.Select(GetApplication).NotNull().ToArray();

        public SteamApplication[] Installed
            => All.Where(a => a.IsInstalled).ToArray();

        public SteamApplication? this[int id]
            => All.FirstOrDefault(a => a.Id == id);

        public SteamApplication? this[string name]
            => All.FirstOrDefault(a => a.Name == name);

        private static SteamApplication? GetApplication(RegistryPath path)
        {
            if (GetApplicationData(path, out int id, out string name))
            {
                return new(id, name, GetInstallationDirectory(id));
            }

            return null;
        }

        private static bool GetApplicationData(RegistryPath path, out int id, out string name)
        {
            if (int.TryParse(path.Name, out id))
            {
                RegistryValues values = RegistryInfo.Read(path).Values;

                name = values.GetString("Name");
            }
            else
            {
                name = string.Empty;
            }

            return name.Length > 0;
        }

        private static DirectoryInfo? GetInstallationDirectory(int id)
        {
            string path = RegistryInfo.Read(UninstallPath.Combine($"Steam App {id}")).Values.GetString("InstallLocation");

            return path.Length > 0 && Directory.Exists(path) ? new(path) : null;
        }
    }
}
