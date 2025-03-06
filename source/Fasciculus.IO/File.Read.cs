using System;
using System.IO;
using System.Text;

namespace Fasciculus.IO
{
    public static partial class FileInfoExtensions
    {
        /// <summary>
        /// Reads all text from the given <paramref name="file"/> using the given <paramref name="encoding"/>.
        /// <para>
        /// The encoding defaults to <see cref="Encoding.UTF8"/>.
        /// </para>
        /// </summary>
        public static string ReadAllText(this FileInfo file, Encoding? encoding = null)
            => File.ReadAllText(file.FullName, encoding.OrUTF8());

        /// <summary>
        /// Reads all lines from the given <paramref name="file"/> using the given <paramref name="encoding"/>.
        /// <para>
        /// The encoding defaults to <see cref="Encoding.UTF8"/>.
        /// </para>
        /// </summary>
        public static string[] ReadAllLines(this FileInfo file, Encoding? encoding = null)
            => File.ReadAllLines(file.FullName, encoding.OrUTF8());

        /// <summary>
        /// Reads all bytes from the given <paramref name="file"/>.
        /// </summary>
        public static byte[] ReadAllBytes(this FileInfo file)
            => File.ReadAllBytes(file.FullName);

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

    }
}