using Fasciculus.IO.Searching;
using System;
using System.IO;
using System.Linq;

namespace Fasciculus.NuGet
{
    public class DirectoryPackages
    {
        public const string DefaultFileName = "Directory.Packages.props";

        public static FileInfo[] Search(string fileName, SearchPath searchPath, bool includeParents = false)
        {
            FileInfo[] files = [.. FileSearch.Search(fileName, searchPath)];
            int count = Math.Max(files.Length, includeParents ? files.Length : 1);

            return [.. files.Take(count)];
        }

        public static FileInfo[] Search(SearchPath searchPath, bool includeParents = false)
            => Search(DefaultFileName, searchPath, includeParents);

        public static FileInfo[] Search(bool includeParents = false)
            => Search(SearchPath.WorkingDirectoryAndParents(), includeParents);
    }
}
