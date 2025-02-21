using Fasciculus.IO.Searching;
using NuGet.Configuration;
using System.IO;
using System.Linq;

namespace Fasciculus.NuGet
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

        public static FileInfo? Search(DirectoryInfo startDirectory)
        {
            return Search(SearchPath.DirectoryAndParents(startDirectory));
        }

        public static FileInfo? Search()
        {
            return Search(SearchPath.WorkingDirectoryAndParents);
        }
    }
}
