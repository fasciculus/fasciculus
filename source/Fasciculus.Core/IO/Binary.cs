using Fasciculus.Support;
using System;
using System.IO;

namespace Fasciculus.IO
{
    /// <summary>
    /// Read and write binary data from or to a stream.
    /// </summary>
    public class Binary
    {
        private readonly Stream stream;

        private readonly byte[] buffer = new byte[16];

        /// <summary>
        /// Initializes a binary reader/writer with the given <paramref name="stream"/>
        /// </summary>
        public Binary(Stream stream)
        {
            this.stream = stream;
        }

        /// <summary>
        /// Implicit creation of a <see cref="Binary"/> from a <see cref="Stream"/>
        /// </summary>
        public static implicit operator Binary(Stream stream)
            => new(stream);

        /// <summary>
        /// Reads a <see cref="short"/> from the stream using the given endianness.
        /// </summary>
        public short ReadShort(Endian endian)
            => endian.GetShort(Read(sizeof(short)));

        /// <summary>
        /// Reads a <see cref="short"/> from the stream using little-endian.
        /// </summary>
        public short ReadShort()
            => ReadShort(Endian.Little);

        /// <summary>
        /// Writes a <see cref="short"/> to the stream using the given endianness.
        /// </summary>
        public void WriteShort(short value, Endian endian)
            => Write(endian.SetShort(buffer, value), sizeof(short));

        /// <summary>
        /// Writes a <see cref="short"/> to the stream using little-endian.
        /// </summary>
        public void WriteShort(short value)
            => WriteShort(value, Endian.Little);

        private byte[] Read(int count)
            => Read(buffer, count);

        private byte[] Read(byte[] buffer, int count)
        {
            if (stream.Read(buffer, 0, count) != count)
            {
                throw Ex.EndOfStream();
            }

            return buffer;
        }

        private void Write(Span<byte> buffer, int count)
        {
            stream.Write(buffer.ToArray(), 0, count);
        }
    }
}
