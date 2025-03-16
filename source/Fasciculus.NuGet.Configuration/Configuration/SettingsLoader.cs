using Fasciculus.IO;
using NuGet.Configuration;
using System.IO;

namespace Fasciculus.NuGet.Configuration
{
    public static class SettingsLoader
    {
        public static ISettings Load()
            => LoadCore(SettingsSearch.Search());

        public static ISettings Load(DirectoryInfo startDirectory)
            => LoadCore(SettingsSearch.Search(startDirectory));

        public static ISettings Load(SearchPath searchPath)
            => LoadCore(SettingsSearch.Search(searchPath));

        private static ISettings LoadCore(DirectoryInfo? directory)
            => Settings.LoadDefaultSettings(directory?.FullName);
    }
}
