using System.IO;

namespace Fasciculus.IO
{
    /// <summary>
    /// Special directories.
    /// </summary>
    public static class SpecialDirectories
    {
        /// <summary>
        /// Home directory.
        /// <para>
        /// Shorthand for <c>Environment.GetFolderPath(SpecialFolder.UserProfile)</c>.
        /// </para>
        /// </summary>
        public static DirectoryInfo Home => new(SpecialPaths.Home);

        /// <summary>
        /// Personal directory.
        /// <para>
        /// Shorthand for <c>Environment.GetFolderPath(SpecialFolder.Personal)</c>.
        /// </para>
        /// </summary>
        public static DirectoryInfo Personal => new(SpecialPaths.Personal);

        /// <summary>
        /// Documents directory.
        /// <para>
        /// Shorthand for <c>Environment.GetFolderPath(SpecialFolder.MyDocuments) on windows</c>.
        /// </para>
        /// <para>
        /// Shorthand for <c>Environment.GetFolderPath(SpecialFolder.Personal) on otherwise</c>.
        /// </para>
        /// </summary>
        public static DirectoryInfo Documents => new(SpecialPaths.Documents);

        /// <summary>
        /// Downloads directory.
        /// <para>
        /// <c><see cref="Home"/>/Downloads</c> on windows
        /// </para>
        /// <para>
        /// <c><see cref="Home"/></c> otherwise
        /// </para>
        /// </summary>
        public static DirectoryInfo Downloads => new(SpecialPaths.Downloads);

        /// <summary>
        /// Program files.
        /// <para>
        /// Shorthand for <c>Environment.GetFolderPath(SpecialFolder.ProgramFiles)</c>.
        /// </para>
        /// </summary>
        public static DirectoryInfo ProgramFiles => new(SpecialPaths.ProgramFiles);

        /// <summary>
        /// Application base directory.
        /// <para>
        /// Shorthand for <c>AppDomain.CurrentDomain.BaseDirectory</c>
        /// </para>
        /// </summary>
        public static DirectoryInfo BaseDirectory => new(SpecialPaths.BaseDirectory);

        /// <summary>
        /// Working directory.
        /// <para>
        /// Shorthand for <c>Environment.CurrentDirectory</c>
        /// </para>
        /// </summary>
        public static DirectoryInfo WorkingDirectory => new(SpecialPaths.WorkingDirectory);
    }
}