using Fasciculus.IO;
using NuGet.Configuration;
using System.IO;
using System.Linq;

namespace Fasciculus.NuGet.Configuration
{
    public static class SettingsSearch
    {
        public static DirectoryInfo? Search(SearchPath searchPath)
        {
            DirectoryInfo? result = null;

            foreach (string name in Settings.OrderedSettingsFileNames)
            {
                result = FileSearch.Search(name, searchPath).FirstOrDefault()?.Directory;

                if (result is not null)
                {
                    break;
                }
            }

            return result;
        }

        public static DirectoryInfo? Search(DirectoryInfo start)
            => Search(SearchPath.DirectoryAndParents(start));

        public static DirectoryInfo? Search()
            => Search(SearchPath.WorkingDirectoryAndParents());
    }
}