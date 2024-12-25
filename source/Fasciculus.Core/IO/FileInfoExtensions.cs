using System.Text;

namespace System.IO
{
    /// <summary>
    /// Extensions for <see cref="FileInfo"/>.
    /// </summary>
    public static class FileInfoExtensions
    {
        /// <summary>
        /// Deletes the given <paramref name="file"/> if it exists.
        /// </summary>
        public static void DeleteIfExists(this FileInfo file)
        {
            if (File.Exists(file.FullName))
            {
                file.Delete();
            }
        }

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
        /// Returns <c>true</c> if:
        /// <list type="bullet">
        /// <item>the given <paramref name="file"/> doesn't exist.</item>
        /// <item>the given <paramref name="file"/> is older than the given <paramref name="dateTimeUtc"/>.</item>
        /// <item>the given <paramref name="mode"/> is <see cref="FileOverwriteMode.Always"/>.</item>
        /// </list>
        /// </summary>
        public static bool RequiresOverwrite(this FileInfo file, DateTime dateTimeUtc, FileOverwriteMode mode)
        {
            file = new FileInfo(file.FullName);

            return mode switch
            {
                FileOverwriteMode.Never => !file.Exists,
                FileOverwriteMode.IfNewer => !file.Exists || file.IsOlderThan(dateTimeUtc),
                FileOverwriteMode.Always => true,
                _ => false
            };
        }

        /// <summary>
        /// Creates the given <paramref name="file"/>'s parent directory if it doesn't exist.
        /// </summary>
        public static FileInfo CreateDirectoryIfNotExists(this FileInfo file)
        {
            file.Directory?.CreateIfNotExists();

            return file;
        }

        /// <summary>
        /// Reads all text from the given <paramref name="file"/> using the given <paramref name="encoding"/>.
        /// <para>
        /// The encoding defaults to <see cref="Encoding.UTF8"/>.
        /// </para>
        /// </summary>
        public static string ReadAllText(this FileInfo file, Encoding? encoding = null)
            => File.ReadAllText(file.FullName, encoding ?? Encoding.UTF8);

        /// <summary>
        /// Writes the given <paramref name="text"/> to the given <paramref name="file"/> using the given <paramref name="encoding"/>.
        /// <para>
        /// The encoding defaults to <see cref="Encoding.UTF8"/>.
        /// </para>
        /// </summary>
        public static void WriteAllText(this FileInfo file, string text, Encoding? encoding = null)
            => File.WriteAllText(file.CreateDirectoryIfNotExists().FullName, text, encoding ?? Encoding.UTF8);

        /// <summary>
        /// Reads all lines from the given <paramref name="file"/> using the given <paramref name="encoding"/>.
        /// <para>
        /// The encoding defaults to <see cref="Encoding.UTF8"/>.
        /// </para>
        /// </summary>
        public static string[] ReadAllLines(this FileInfo file, Encoding? encoding = null)
            => File.ReadAllLines(file.FullName, encoding ?? Encoding.UTF8);

        /// <summary>
        /// Reads all bytes from the given <paramref name="file"/>.
        /// </summary>
        public static byte[] ReadAllBytes(this FileInfo file)
            => File.ReadAllBytes(file.FullName);

        /// <summary>
        /// Writes the given <paramref name="bytes"/> to the given <paramref name="file"/>. 
        /// </summary>
        public static void WriteAllBytes(this FileInfo file, byte[] bytes)
            => File.WriteAllBytes(file.CreateDirectoryIfNotExists().FullName, bytes);

        /// <summary>
        /// Opens the given <paramref name="file"/> for reading and calls the given <paramref name="read"/> action to perform the actual
        /// read.
        /// <para>
        /// The file is auto-closed after the read operation.
        /// </para>
        /// </summary>
        public static void Read(this FileInfo file, Action<Stream> read)
        {
            using Stream stream = file.OpenRead();

            read(stream);
        }

        /// <summary>
        /// Opens the given <paramref name="file"/> for reading and calls the given <paramref name="read"/> function to perform the actual
        /// read.
        /// <para>
        /// The file is auto-closed after the read operation.
        /// </para>
        /// </summary>
        /// <returns>The <paramref name="read"/>'s return value.</returns>
        public static T Read<T>(this FileInfo file, Func<Stream, T> read)
        {
            using Stream stream = file.OpenRead();

            return read(stream);
        }

        /// <summary>
        /// Opens the given <paramref name="file"/> for writing and calls the given <paramref name="write"/> action to perform the actual
        /// write.
        /// <para>
        /// The file is auto-closed after the write operation.
        /// </para>
        /// <para>
        /// The file is deleted first if it exists.
        /// </para>
        /// </summary>
        public static FileInfo Write(this FileInfo file, Action<Stream> write)
        {
            file.DeleteIfExists();

            using (Stream stream = file.CreateDirectoryIfNotExists().Create())
            {
                write(stream);
            }

            return new(file.FullName);
        }
    }
}
