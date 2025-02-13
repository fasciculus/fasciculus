using System;
using System.IO;
using System.Runtime.InteropServices;
using static System.Environment;

namespace Fasciculus.IO
{
    /// <summary>
    /// Special paths.
    /// </summary>
    public static class SpecialPaths
    {
        /// <summary>
        /// Home directory.
        /// <para>
        /// Shorthand for <c>Environment.GetFolderPath(SpecialFolder.UserProfile)</c>.
        /// </para>
        /// </summary>
        public static string Home => GetFolderPath(SpecialFolder.UserProfile);

        /// <summary>
        /// Personal directory.
        /// <para>
        /// Shorthand for <c>Environment.GetFolderPath(SpecialFolder.Personal)</c>.
        /// </para>
        /// </summary>
        public static string Personal => GetFolderPath(SpecialFolder.Personal);

        /// <summary>
        /// Documents directory.
        /// <para>
        /// Shorthand for <c>Environment.GetFolderPath(SpecialFolder.MyDocuments) on windows</c>.
        /// </para>
        /// <para>
        /// Shorthand for <c>Environment.GetFolderPath(SpecialFolder.Personal) on otherwise</c>.
        /// </para>
        /// </summary>
        public static string Documents => GetFolderPath(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? SpecialFolder.MyDocuments : SpecialFolder.Personal);

        /// <summary>
        /// Downloads directory.
        /// <para>
        /// <c><see cref="Home"/>/Downloads</c> on windows
        /// </para>
        /// <para>
        /// <c><see cref="Home"/></c> otherwise
        /// </para>
        /// </summary>
        public static string Downloads => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? Path.Combine(Home, "Downloads") : Home;

        /// <summary>
        /// Program files.
        /// <para>
        /// Shorthand for <c>Environment.GetFolderPath(SpecialFolder.ProgramFiles)</c>.
        /// </para>
        /// </summary>
        public static string ProgramFiles => GetFolderPath(SpecialFolder.ProgramFiles);

        /// <summary>
        /// Application base directory.
        /// <para>
        /// Shorthand for <c>AppDomain.CurrentDomain.BaseDirectory</c>
        /// </para>
        /// </summary>
        public static string BaseDirectory => AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// Working directory.
        /// <para>
        /// Shorthand for <c>Environment.CurrentDirectory</c>
        /// </para>
        /// </summary>
        public static string WorkingDirectory => CurrentDirectory;
    }
}
