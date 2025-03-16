using Fasciculus.IO;
using Fasciculus.Threading;
using NuGet.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace Fasciculus.NuGet.Configuration
{
    public static class SettingsLoader
    {
        public static AsyncLazy<IMachineWideSettings> MachineWideSettings = new(LoadMachineWideSettings);

        private static IMachineWideSettings LoadMachineWideSettings()
            => new XPlatMachineWideSetting();

        public static async Task<ISettings> LoadAsync()
            => await LoadSettingsAsync(SettingsSearch.Search());

        public static async Task<ISettings> LoadAsync(DirectoryInfo startDirectory)
            => await LoadSettingsAsync(SettingsSearch.Search(startDirectory));

        public static async Task<ISettings> LoadAsync(SearchPath searchPath)
            => await LoadSettingsAsync(SettingsSearch.Search(searchPath));

        public static async Task<ISettings> LoadAsync(FileInfo file)
            => await LoadSettingsAsync(file.DirectoryName, file.Name);

        private static async Task<ISettings> LoadSettingsAsync(DirectoryInfo? directory)
            => await LoadSettingsAsync(directory?.FullName, null);

        private static async Task<ISettings> LoadSettingsAsync(string? root, string? fileName)
        {
            IMachineWideSettings machineWideSettings = await MachineWideSettings;

            return Settings.LoadDefaultSettings(root, fileName, machineWideSettings);
        }
    }
}
