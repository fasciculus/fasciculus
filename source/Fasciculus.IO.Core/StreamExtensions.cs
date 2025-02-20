using System;
using System.IO;
using System.Text;

namespace Fasciculus.IO
{
    /// <summary>
    /// Extensions for <see cref="Stream"/>.
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Reads the remaining bytes from the current position of the stream to its end.
        /// </summary>
        /// <remarks>
        /// Fails if more than <c>int.MaxValue</c> bytes must be read.
        /// </remarks>
        public static byte[] ReadAllBytes(this Stream stream)
        {
            long count = stream.Length - stream.Position;

            if (count > int.MaxValue)
            {
                throw new InvalidOperationException("stream too big");
            }

            byte[] result = new byte[count];

            stream.ReadExactly(result);

            return result;
        }

        /// <summary>
        /// Reads the remaining bytes from the current position of the stream to its end and converts them into a string.
        /// </summary>
        public static string ReadAllText(this Stream stream, Encoding encoding)
            => encoding.GetString(ReadAllBytes(stream));

        /// <summary>
        /// Reads the remaining bytes from the current position of the stream to its end and converts them into a string using UTF-8.
        /// </summary>
        public static string ReadAllText(this Stream stream)
            => stream.ReadAllText(Encoding.UTF8);

        /// <summary>
        /// Converts the given string to bytes using the given encoding and writes them to the stream.
        /// </summary>
        public static void WriteAllText(this Stream stream, string text, Encoding encoding)
        {
            byte[] bytes = encoding.GetBytes(text);

            stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Converts the given string to bytes using UTF-8 encoding and writes them to the stream.
        /// </summary>
        public static void WriteAllText(this Stream stream, string text)
            => stream.WriteAllText(text, Encoding.UTF8);
    }
}
