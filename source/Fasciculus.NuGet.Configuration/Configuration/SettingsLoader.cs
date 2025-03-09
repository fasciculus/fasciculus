using Fasciculus.IO;
using NuGet.Configuration;
using System.IO;

namespace Fasciculus.NuGet.Configuration
{
    public static class SettingsLoader
    {
        public static ISettings Load()
            => Load(SettingsSearch.Search());

        public static ISettings Load(DirectoryInfo startDirectory)
            => Load(SettingsSearch.Search(startDirectory));

        public static ISettings Load(SearchPath searchPath)
            => Load(SettingsSearch.Search(searchPath));

        public static ISettings Load(FileInfo? file)
            => Settings.LoadDefaultSettings(file?.Directory?.FullName);
    }
}
