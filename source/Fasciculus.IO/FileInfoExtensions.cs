using Fasciculus.Algorithms.Comparing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Fasciculus.IO
{
    /// <summary>
    /// Extensions for <see cref="FileInfo"/>.
    /// </summary>
    public static class FileInfoExtensions
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
        /// Deletes the given <paramref name="file"/> if it exists.
        /// </summary>
        public static FileInfo DeleteIfExists(this FileInfo file)
        {
            if (file.Update().Exists)
            {
                file.Delete();
            }

            return file.Update();
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
        /// <li>the given <paramref name="mode"/> is <see cref="FileOverwriteMode.Always"/>.</li>
        /// </ul>
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

            return file.Update();
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
        public static FileInfo WriteAllText(this FileInfo file, string text, Encoding? encoding = null)
        {
            File.WriteAllText(file.CreateDirectoryIfNotExists().DeleteIfExists().FullName, text, encoding ?? Encoding.UTF8);

            return file.Update();
        }

        /// <summary>
        /// Reads all lines from the given <paramref name="file"/> using the given <paramref name="encoding"/>.
        /// <para>
        /// The encoding defaults to <see cref="Encoding.UTF8"/>.
        /// </para>
        /// </summary>
        public static string[] ReadAllLines(this FileInfo file, Encoding? encoding = null)
            => File.ReadAllLines(file.FullName, encoding ?? Encoding.UTF8);

        /// <summary>
        /// Writes the given <paramref name="lines"/> to the given <paramref name="file"/> using the given <paramref name="encoding"/>.
        /// </summary>
        public static FileInfo WriteAllLines(this FileInfo file, string[] lines, Encoding? encoding = null)
        {
            File.WriteAllLines(file.CreateDirectoryIfNotExists().DeleteIfExists().FullName, lines, encoding ?? Encoding.UTF8);

            return file.Update();
        }

        /// <summary>
        /// Writes the given <paramref name="lines"/> to the given <paramref name="file"/> using the given <paramref name="encoding"/>.
        /// </summary>
        public static FileInfo WriteAllLines(this FileInfo file, IEnumerable<string> lines, Encoding? encoding = null)
        {
            File.WriteAllLines(file.CreateDirectoryIfNotExists().DeleteIfExists().FullName, lines, encoding ?? Encoding.UTF8);

            return file.Update();
        }

        /// <summary>
        /// Reads all bytes from the given <paramref name="file"/>.
        /// </summary>
        public static byte[] ReadAllBytes(this FileInfo file)
            => File.ReadAllBytes(file.FullName);

        /// <summary>
        /// Writes the given <paramref name="bytes"/> to the given <paramref name="file"/>. 
        /// </summary>
        public static FileInfo WriteAllBytes(this FileInfo file, byte[] bytes)
        {
            File.WriteAllBytes(file.CreateDirectoryIfNotExists().DeleteIfExists().FullName, bytes);

            return file.Update();
        }

        /// <summary>
        /// Opens the given <paramref name="file"/> for reading and calls the given <paramref name="read"/> action to perform the actual
        /// read.
        /// <para>
        /// The file is auto-closed after the read operation.
        /// </para>
        /// </summary>
        public static FileInfo Read(this FileInfo file, Action<Stream> read)
        {
            using Stream stream = file.OpenRead();

            read(stream);

            return file;
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

            using (Stream stream = file.CreateDirectoryIfNotExists().DeleteIfExists().Create())
            {
                write(stream);
            }

            return file.Update();
        }

        /// <summary>
        /// Returns <c>true</c> if:
        /// <ul>
        /// <li>the <paramref name="file"/> doesn't exist.</li>
        /// <li>the <paramref name="file"/> has not the same length as the given <paramref name="bytes"/>.</li>
        /// <li>the <paramref name="file"/>'s content differs from the given <paramref name="bytes"/>.</li>
        /// </ul>
        /// </summary>
        public static bool IsDifferent(this FileInfo file, byte[] bytes)
        {
            if (!file.Update().Exists || file.Length != bytes.Length)
            {
                return true;
            }

            byte[] existing = file.ReadAllBytes();

            return !ByteArrayEqualityComparer.AreEqual(existing, bytes);
        }

        /// <summary>
        /// Writes the given <paramref name="bytes"/> to the given <paramref name="file"/> if
        /// <see cref="IsDifferent(FileInfo, byte[])"/> returns <c>true</c>.
        /// </summary>
        public static FileInfo WriteIfDifferent(this FileInfo file, byte[] bytes, out bool written)
        {
            written = false;

            if (file.IsDifferent(bytes))
            {
                file.WriteAllBytes(bytes);
                written = true;
            }

            return file.Update();
        }

        /// <summary>
        /// Writes the given <paramref name="bytes"/> to the given <paramref name="file"/> if
        /// <see cref="IsDifferent(FileInfo, byte[])"/> returns <c>true</c>.
        /// </summary>
        public static FileInfo WriteIfDifferent(this FileInfo file, byte[] bytes)
            => file.WriteIfDifferent(bytes, out bool _);
    }
}
