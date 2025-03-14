using Fasciculus.IO;
using Fasciculus.Threading;
using NuGet.Configuration;
using System.IO;

namespace Fasciculus.NuGet.Configuration
{
    public class NuGetSettings
    {
        public static AsyncLazy<NuGetSettings> Default = new(() => new NuGetSettings());

        public ISettings Settings { get; }

        public NuGetSettings(ISettings settings)
        {
            Settings = settings;
        }

        public NuGetSettings()
            : this(SettingsLoader.Load()) { }

        public NuGetSettings(DirectoryInfo startDirectory)
            : this(SettingsLoader.Load(startDirectory)) { }

        public NuGetSettings(SearchPath searchPath)
            : this(SettingsLoader.Load(searchPath)) { }

        public NuGetSettings(FileInfo? file)
            : this(SettingsLoader.Load(file)) { }
    }
}