using System.Collections.Generic;
using System.IO;

namespace Fasciculus.IO
{
    /// <summary>
    /// Search for directories.
    /// </summary>
    public static class DirectorySearch
    {
        /// <summary>
        /// Searches for directories matching the given <paramref name="pattern"/> in the given <paramref name="searchPath"/>.
        /// </summary>
        public static IEnumerable<DirectoryInfo> Search(string pattern, SearchPath searchPath)
        {
            foreach (DirectoryInfo searchDirectory in searchPath)
            {
                foreach (DirectoryInfo directory in searchDirectory.EnumerateDirectories(pattern, SearchOption.TopDirectoryOnly))
                {
                    yield return directory;
                }
            }
        }
    }
}
