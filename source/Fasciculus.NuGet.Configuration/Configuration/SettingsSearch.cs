using Fasciculus.IO;
using NuGet.Configuration;
using System.IO;
using System.Linq;

namespace Fasciculus.NuGet.Configuration
{
    public static class SettingsSearch
    {
        public static FileInfo? Search(SearchPath searchPath)
        {
            FileInfo? result = null;

            foreach (string name in Settings.OrderedSettingsFileNames)
            {
                result = FileSearch.Search(name, searchPath).FirstOrDefault();

                if (result is not null)
                {
                    break;
                }
            }

            return result;
        }

        public static FileInfo? Search(DirectoryInfo start)
            => Search(SearchPath.DirectoryAndParents(start));

        public static FileInfo? Search()
            => Search(SearchPath.WorkingDirectoryAndParents());
    }
}
