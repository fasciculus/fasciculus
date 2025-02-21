using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.IO
{
    /// <summary>
    /// Extensions for <see cref="DirectoryInfo"/>.
    /// </summary>
    public static class DirectoryInfoExtensions
    {
        /// <summary>
        /// Adds the given <paramref name="name"/> to the given <paramref name="directory"/> and returns the resulting <see cref="FileInfo"/>.
        /// </summary>
        public static FileInfo File(this DirectoryInfo directory, string name)
            => new(Path.Combine(directory.FullName, name));

        /// <summary>
        /// Creates the given <paramref name="directory"/> if it doesn't exist yet.
        /// </summary>
        /// <returns></returns>
        public static DirectoryInfo CreateIfNotExists(this DirectoryInfo directory)
        {
            DirectoryInfo result = new(directory.FullName);

            if (!result.Exists)
            {
                result.Create();
                result = new(directory.FullName);
            }

            return result;
        }

        /// <summary>
        /// Adds the path elements given in <paramref name="paths"/> to the given <paramref name="directory"/> and returns the resulting
        /// <see cref="DirectoryInfo"/>.
        /// </summary>
        public static DirectoryInfo Combine(this DirectoryInfo directory, params string[] paths)
        {
            string result = directory.FullName;

            foreach (string path in paths)
            {
                result = Path.Combine(result, path);
            }

            return new DirectoryInfo(result);
        }

        /// <summary>
        /// Returns the parents of the given <paramref name="directory"/>, nearest first.
        /// </summary>
        public static DirectoryInfo[] GetParents(this DirectoryInfo directory)
            => [.. GetParentsCore(directory)];

        /// <summary>
        /// Returns the given <paramref name="directory"/> and its parents, nearest (i.e. <paramref name="directory"/>) first.
        /// </summary>
        public static DirectoryInfo[] GetSelfAndParents(this DirectoryInfo directory)
            => [.. GetParentsCore(directory).Prepend(directory)];

        private static IEnumerable<DirectoryInfo> GetParentsCore(DirectoryInfo directory)
        {
            for (DirectoryInfo current = directory.Parent; current != null; current = current.Parent)
            {
                yield return current;
            }
        }
    }
}
