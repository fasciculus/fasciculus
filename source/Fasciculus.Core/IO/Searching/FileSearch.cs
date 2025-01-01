using System.Collections.Generic;
using System.IO;

namespace Fasciculus.IO.Searching
{
    /// <summary>
    /// Search for files.
    /// </summary>
    public static class FileSearch
    {
        /// <summary>
        /// Searches for files matching the given <paramref name="pattern"/> in the given <paramref name="searchPath"/>.
        /// </summary>
        public static IEnumerable<FileInfo> Search(string pattern, SearchPath searchPath)
        {
            foreach (DirectoryInfo searchDirectory in searchPath)
            {
                foreach (FileInfo file in searchDirectory.EnumerateFiles(pattern, SearchOption.TopDirectoryOnly))
                {
                    yield return file;
                }
            }
        }
    }
}
