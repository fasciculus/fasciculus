using Fasciculus.IO;
using Fasciculus.Threading;
using NuGet.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace Fasciculus.NuGet.Configuration
{
    public class NuGetSettings
    {
        public static AsyncLazy<NuGetSettings> Default = new(LoadAsync);

        public ISettings Settings { get; }

        public NuGetSettings(ISettings settings)
        {
            Settings = settings;
        }

        public static async Task<NuGetSettings> LoadAsync()
            => new(await SettingsLoader.LoadAsync());

        public static async Task<NuGetSettings> Load(DirectoryInfo startDirectory)
            => new(await SettingsLoader.LoadAsync(startDirectory));

        public static async Task<NuGetSettings> Load(SearchPath searchPath)
            => new(await SettingsLoader.LoadAsync(searchPath));

        public static async Task<NuGetSettings> LoadSpecific(FileInfo file)
            => new(await SettingsLoader.LoadAsync(file));
    }
}