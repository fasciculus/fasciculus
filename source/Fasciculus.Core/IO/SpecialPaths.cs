using Fasciculus.Interop;
using System;
using System.IO;
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
        public static string Documents => GetFolderPath(OS.IsWindows ? SpecialFolder.MyDocuments : SpecialFolder.Personal);

        /// <summary>
        /// Downloads directory.
        /// <para>
        /// <c><see cref="Home"/>/Downloads</c> on windows
        /// </para>
        /// <para>
        /// <c><see cref="Home"/></c> otherwise
        /// </para>
        /// </summary>
        public static string Downloads => OS.IsWindows ? Path.Combine(Home, "Downloads") : Home;

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
