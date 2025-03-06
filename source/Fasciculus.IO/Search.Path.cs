using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Fasciculus.IO
{
    /// <summary>
    /// List of directories to search in.
    /// </summary>
    public class SearchPath : IEnumerable<DirectoryInfo>
    {
        private readonly List<DirectoryInfo> directories = [];

        /// <summary>
        /// Initializes an empty search path.
        /// </summary>
        public SearchPath() { }

        /// <summary>
        /// Initializes a search path with the given <paramref name="directories"/>.
        /// </summary>
        public SearchPath(IEnumerable<DirectoryInfo> directories, bool recursive = false)
        {
            Add(directories, recursive);
        }

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
                foreach (DirectoryInfo subDirectory in directory.GetDirectories())
                {
                    Add(subDirectory, true);
                }
            }

            return this;
        }

        /// <summary>
        /// Adds the given <paramref name="directories"/>. Recursively adds their subdirectories, if <paramref name="recursive"/> is <c>true</c>.
        /// </summary>
        public SearchPath Add(IEnumerable<DirectoryInfo> directories, bool recursive = false)
        {
            foreach (DirectoryInfo directory in directories)
            {
                Add(directory, recursive);
            }

            return this;
        }

        /// <summary>
        /// Returns a new empty search path.
        /// </summary>
        public static SearchPath Empty()
            => [];

        /// <summary>
        /// Returns the working directory and its parents.
        /// </summary>
        public static SearchPath WorkingDirectoryAndParents()
            => new(SpecialDirectories.WorkingDirectory.GetSelfAndParents());

        /// <summary>
        /// Returns the given <paramref name="directory"/> and its parents.
        /// </summary>
        /// <param name="directory"></param>
        public static SearchPath DirectoryAndParents(DirectoryInfo directory)
            => new(directory.GetSelfAndParents());
    }
}
