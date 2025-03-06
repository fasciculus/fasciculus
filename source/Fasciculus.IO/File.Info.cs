using System;
using System.IO;

namespace Fasciculus.IO
{
    /// <summary>
    /// Extensions for <see cref="FileInfo"/>.
    /// </summary>
    public static partial class FileInfoExtensions
    {
        /// <summary>
        /// Calls <c>file.Refresh()</c> and returns the same file.
        /// </summary>
        public static FileInfo Update(this FileInfo file)
        {
            file.Refresh();

            return file;
        }

        /// <summary>
        /// Returns the name of the given <paramref name="file"/> stripped from its extension.
        /// </summary>
        public static string NameWithoutExtension(this FileInfo file)
            => Path.GetFileNameWithoutExtension(file.Name);

        /// <summary>
        /// Returns whether given <paramref name="file"/>'s last write time is newer than the given <paramref name="lastWriteTimeUtc"/>.
        /// </summary>
        public static bool IsNewerThan(this FileInfo file, DateTime lastWriteTimeUtc)
            => file.LastWriteTimeUtc > lastWriteTimeUtc;

        /// <summary>
        /// Returns whether given <paramref name="file"/>'s last write time is newer than the given <paramref name="target"/>'s
        /// last write time.
        /// </summary>
        public static bool IsNewerThan(this FileInfo file, FileInfo target)
            => !target.Exists || file.IsNewerThan(target.LastWriteTimeUtc);

        /// <summary>
        /// Returns whether given <paramref name="file"/>'s last write time is older than the given <paramref name="lastWriteTimeUtc"/>.
        /// </summary>
        public static bool IsOlderThan(this FileInfo file, DateTime lastWriteTimeUtc)
            => file.LastWriteTimeUtc < lastWriteTimeUtc;

        /// <summary>
        /// Returns whether given <paramref name="file"/>'s last write time is older than the given <paramref name="target"/>'s
        /// last write time.
        /// </summary>
        public static bool IsOlderThan(this FileInfo file, FileInfo target)
            => !target.Exists || file.IsOlderThan(target.LastWriteTimeUtc);

        /// <summary>
        /// Returns <c>true</c> if:
        /// <ul>
        /// <li>the given <paramref name="file"/> doesn't exist.</li>
        /// <li>the given <paramref name="file"/> is older than the given <paramref name="dateTimeUtc"/>.</li>
        /// <li>the given <paramref name="mode"/> is <see cref="FileOverwrite.Always"/>.</li>
        /// </ul>
        /// </summary>
        public static bool RequiresOverwrite(this FileInfo file, DateTime dateTimeUtc, FileOverwrite mode)
        {
            file = new FileInfo(file.FullName);

            return mode switch
            {
                FileOverwrite.Never => !file.Exists,
                FileOverwrite.IfNewer => !file.Exists || file.IsOlderThan(dateTimeUtc),
                FileOverwrite.Always => true,
                _ => false
            };
        }
    }
}