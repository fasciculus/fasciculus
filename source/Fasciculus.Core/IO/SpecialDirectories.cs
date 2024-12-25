using System.IO;

namespace Fasciculus.IO
{
    /// <summary>
    /// Special directories.
    /// </summary>
    public static class SpecialDirectories
    {
        /// <summary>
        /// <see cref="DirectoryInfo"/> for <see cref="SpecialPaths.Home"/>.
        /// </summary>
        public static DirectoryInfo Home => new(SpecialPaths.Home);

        /// <summary>
        /// <see cref="DirectoryInfo"/> for <see cref="SpecialPaths.Personal"/>.
        /// </summary>
        public static DirectoryInfo Personal => new(SpecialPaths.Personal);

        /// <summary>
        /// <see cref="DirectoryInfo"/> for <see cref="SpecialPaths.Documents"/>.
        /// </summary>
        public static DirectoryInfo Documents => new(SpecialPaths.Documents);

        /// <summary>
        /// <see cref="DirectoryInfo"/> for <see cref="SpecialPaths.Downloads"/>.
        /// </summary>
        public static DirectoryInfo Downloads => new(SpecialPaths.Downloads);

        /// <summary>
        /// <see cref="DirectoryInfo"/> for <see cref="SpecialPaths.BaseDirectory"/>.
        /// </summary>
        public static DirectoryInfo BaseDirectory => new(SpecialPaths.BaseDirectory);
    }
}