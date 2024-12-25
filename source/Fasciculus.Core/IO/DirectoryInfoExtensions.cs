namespace System.IO
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
    }
}
