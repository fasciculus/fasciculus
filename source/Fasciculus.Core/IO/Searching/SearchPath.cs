using Fasciculus.Collections;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Fasciculus.IO.Searching
{
    /// <summary>
    /// List of directories to search in.
    /// </summary>
    public class SearchPath : IEnumerable<DirectoryInfo>
    {
        private readonly List<DirectoryInfo> directories = [];

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        public IEnumerator<DirectoryInfo> GetEnumerator()
            => directories.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => directories.GetEnumerator();

        /// <summary>
        /// Adds the given <paramref name="directory"/>. Recursively adds its subdirectories, if <paramref name="recursive"/> is <c>true</c>.
        /// </summary>
        public SearchPath Add(DirectoryInfo directory, bool recursive = false)
        {
            directories.Add(directory);

            if (recursive)
            {
                directory.GetDirectories().Apply(x => { Add(x, true); });
            }

            return this;
        }

        /// <summary>
        /// Adds the working directory and its parents.
        /// </summary>
        public SearchPath AddWorkingDirectoryAndParents()
        {
            DirectoryInfo? directory = SpecialDirectories.WorkingDirectory;

            while (directory is not null)
            {
                Add(directory);
                directory = directory.Parent;
            }

            return this;
        }

        /// <summary>
        /// Returns a new empty search path.
        /// </summary>
        public static SearchPath Empty
            => [];

        /// <summary>
        /// Returns the working directory and its parents.
        /// </summary>
        public static SearchPath WorkingDirectoryAndParents
            => Empty.AddWorkingDirectoryAndParents();
    }
}
