using Fasciculus.Algorithms.Comparing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Fasciculus.IO
{
    public static partial class FileInfoExtensions
    {
        /// <summary>
        /// Writes the given <paramref name="text"/> to the given <paramref name="file"/> using the given <paramref name="encoding"/>.
        /// <para>
        /// The encoding defaults to <see cref="Encoding.UTF8"/>.
        /// </para>
        /// </summary>
        public static FileInfo WriteAllText(this FileInfo file, string text, Encoding? encoding = null)
        {
            File.WriteAllText(file.DeleteIfExists().EnsureDirectory().FullName, text, encoding.OrUTF8());

            return file.Update();
        }

        /// <summary>
        /// Writes the given <paramref name="lines"/> to the given <paramref name="file"/> using the given <paramref name="encoding"/>.
        /// </summary>
        public static FileInfo WriteAllLines(this FileInfo file, string[] lines, Encoding? encoding = null)
        {
            File.WriteAllLines(file.DeleteIfExists().EnsureDirectory().FullName, lines, encoding.OrUTF8());

            return file.Update();
        }

        /// <summary>
        /// Writes the given <paramref name="lines"/> to the given <paramref name="file"/> using the given <paramref name="encoding"/>.
        /// </summary>
        public static FileInfo WriteAllLines(this FileInfo file, IEnumerable<string> lines, Encoding? encoding = null)
        {
            File.WriteAllLines(file.DeleteIfExists().EnsureDirectory().FullName, lines, encoding ?? Encoding.UTF8);

            return file.Update();
        }

        /// <summary>
        /// Writes the given <paramref name="bytes"/> to the given <paramref name="file"/>. 
        /// </summary>
        public static FileInfo WriteAllBytes(this FileInfo file, byte[] bytes)
        {
            File.WriteAllBytes(file.DeleteIfExists().EnsureDirectory().FullName, bytes);

            return file.Update();
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

            using (Stream stream = file.DeleteIfExists().EnsureDirectory().Create())
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