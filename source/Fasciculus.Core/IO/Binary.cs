using Fasciculus.Support;
using System;
using System.IO;
using System.Text;

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
        /// Reads a <see cref="byte"/> from the stream
        /// </summary>
        /// <returns></returns>
        public byte ReadByte()
            => ReadCore(1)[0];

        /// <summary>
        /// Writes a <see cref="byte"/> to the stream.
        /// </summary>
        public void WriteByte(byte value)
            => WriteCore([value], 1);

        /// <summary>
        /// Reads a <see cref="sbyte"/> from the stream
        /// </summary>
        public sbyte ReadSByte()
            => (sbyte)ReadByte();

        /// <summary>
        /// Writes a <see cref="sbyte"/> to the stream.
        /// </summary>
        public void WriteSByte(sbyte value)
            => WriteByte((byte)value);

        /// <summary>
        /// Reads a <see cref="short"/> from the stream using the given endianness.
        /// </summary>
        public short ReadShort(Endian endian)
            => endian.GetShort(ReadCore(sizeof(short)));

        /// <summary>
        /// Reads a <see cref="short"/> from the stream using little-endian.
        /// </summary>
        public short ReadShort()
            => ReadShort(Endian.Little);

        /// <summary>
        /// Writes a <see cref="short"/> to the stream using the given endianness.
        /// </summary>
        public void WriteShort(short value, Endian endian)
            => WriteCore(endian.SetShort(buffer, value), sizeof(short));

        /// <summary>
        /// Writes a <see cref="short"/> to the stream using little-endian.
        /// </summary>
        public void WriteShort(short value)
            => WriteShort(value, Endian.Little);

        /// <summary>
        /// Reads a <see cref="ushort"/> from the stream using the given endianness.
        /// </summary>
        public ushort ReadUShort(Endian endian)
            => endian.GetUShort(ReadCore(sizeof(short)));

        /// <summary>
        /// Reads a <see cref="ushort"/> from the stream using little-endian.
        /// </summary>
        public ushort ReadUShort()
            => ReadUShort(Endian.Little);

        /// <summary>
        /// Writes a <see cref="ushort"/> to the stream using the given endianness.
        /// </summary>
        public void WriteUShort(ushort value, Endian endian)
            => WriteCore(endian.SetUShort(buffer, value), sizeof(short));

        /// <summary>
        /// Writes a <see cref="ushort"/> to the stream using little-endian.
        /// </summary>
        public void WriteUShort(ushort value)
            => WriteUShort(value, Endian.Little);

        /// <summary>
        /// Reads a <see cref="int"/> from the stream using the given endianness.
        /// </summary>
        public int ReadInt(Endian endian)
            => endian.GetInt(ReadCore(sizeof(int)));

        /// <summary>
        /// Reads a <see cref="int"/> from the stream using little-endian.
        /// </summary>
        public int ReadInt()
            => ReadInt(Endian.Little);

        /// <summary>
        /// Writes a <see cref="int"/> to the stream using the given endianness.
        /// </summary>
        public void WriteInt(int value, Endian endian)
            => WriteCore(endian.SetInt(buffer, value), sizeof(int));

        /// <summary>
        /// Writes a <see cref="int"/> to the stream using little-endian.
        /// </summary>
        public void WriteInt(int value)
            => WriteInt(value, Endian.Little);

        /// <summary>
        /// Reads a <see cref="uint"/> from the stream using the given endianness.
        /// </summary>
        public uint ReadUInt(Endian endian)
            => endian.GetUInt(ReadCore(sizeof(uint)));

        /// <summary>
        /// Reads a <see cref="uint"/> from the stream using little-endian.
        /// </summary>
        public uint ReadUInt()
            => ReadUInt(Endian.Little);

        /// <summary>
        /// Writes a <see cref="uint"/> to the stream using the given endianness.
        /// </summary>
        public void WriteUInt(uint value, Endian endian)
            => WriteCore(endian.SetUInt(buffer, value), sizeof(uint));

        /// <summary>
        /// Writes a <see cref="uint"/> to the stream using little-endian.
        /// </summary>
        public void WriteUInt(uint value)
            => WriteUInt(value, Endian.Little);

        /// <summary>
        /// Reads a <see cref="long"/> from the stream using the given endianness.
        /// </summary>
        public long ReadLong(Endian endian)
            => endian.GetLong(ReadCore(sizeof(long)));

        /// <summary>
        /// Reads a <see cref="long"/> from the stream using little-endian.
        /// </summary>
        public long ReadLong()
            => ReadLong(Endian.Little);

        /// <summary>
        /// Writes a <see cref="long"/> to the stream using the given endianness.
        /// </summary>
        public void WriteLong(long value, Endian endian)
            => WriteCore(endian.SetLong(buffer, value), sizeof(long));

        /// <summary>
        /// Writes a <see cref="long"/> to the stream using little-endian.
        /// </summary>
        public void WriteLong(long value)
            => WriteLong(value, Endian.Little);

        /// <summary>
        /// Reads a <see cref="ulong"/> from the stream using the given endianness.
        /// </summary>
        public ulong ReadULong(Endian endian)
            => endian.GetULong(ReadCore(sizeof(ulong)));

        /// <summary>
        /// Reads a <see cref="ulong"/> from the stream using little-endian.
        /// </summary>
        public ulong ReadULong()
            => ReadULong(Endian.Little);

        /// <summary>
        /// Writes a <see cref="ulong"/> to the stream using the given endianness.
        /// </summary>
        public void WriteULong(ulong value, Endian endian)
            => WriteCore(endian.SetULong(buffer, value), sizeof(ulong));

        /// <summary>
        /// Writes a <see cref="ulong"/> to the stream using little-endian.
        /// </summary>
        public void WriteULong(ulong value)
            => WriteULong(value, Endian.Little);

        /// <summary>
        /// Reads a <see cref="float"/> from the stream using the given endianness.
        /// </summary>
        public float ReadFloat(Endian endian)
            => endian.GetFloat(ReadCore(sizeof(float)));

        /// <summary>
        /// Reads a <see cref="float"/> from the stream using little-endian.
        /// </summary>
        public float ReadFloat()
            => ReadFloat(Endian.Little);

        /// <summary>
        /// Writes a <see cref="float"/> to the stream using the given endianness.
        /// </summary>
        public void WriteFloat(float value, Endian endian)
            => WriteCore(endian.SetFloat(buffer, value), sizeof(float));

        /// <summary>
        /// Writes a <see cref="float"/> to the stream using little-endian.
        /// </summary>
        public void WriteFloat(float value)
            => WriteFloat(value, Endian.Little);

        /// <summary>
        /// Reads a <see cref="double"/> from the stream using the given endianness.
        /// </summary>
        public double ReadDouble(Endian endian)
            => endian.GetDouble(ReadCore(sizeof(double)));

        /// <summary>
        /// Reads a <see cref="double"/> from the stream using little-endian.
        /// </summary>
        public double ReadDouble()
            => ReadDouble(Endian.Little);

        /// <summary>
        /// Writes a <see cref="double"/> to the stream using the given endianness.
        /// </summary>
        public void WriteDouble(double value, Endian endian)
            => WriteCore(endian.SetDouble(buffer, value), sizeof(double));

        /// <summary>
        /// Writes a <see cref="double"/> to the stream using little-endian.
        /// </summary>
        public void WriteDouble(double value)
            => WriteDouble(value, Endian.Little);

        /// <summary>
        /// Reads a <see cref="DateTime"/> from the stream using the given endianness.
        /// </summary>
        public DateTime ReadDateTime(Endian endian)
            => DateTime.FromBinary(ReadLong(endian));

        /// <summary>
        /// Reads a <see cref="DateTime"/> from the stream using little-endian.
        /// </summary>
        public DateTime ReadDateTime()
            => ReadDateTime(Endian.Little);

        /// <summary>
        /// Writes a <see cref="DateTime"/> to the stream using the given endianness.
        /// </summary>
        public void WriteDateTime(DateTime value, Endian endian)
            => WriteLong(value.ToBinary(), endian);

        /// <summary>
        /// Writes a <see cref="DateTime"/> to the stream using little-endian.
        /// </summary>
        public void WriteDateTime(DateTime value)
            => WriteDateTime(value, Endian.Little);

        /// <summary>
        /// Reads a <see cref="string"/> from the stream using the given endianness.
        /// </summary>
        public string ReadString(Endian endian)
        {
            int length = ReadInt(endian);
            byte[] bytes = new byte[length];

            ReadCore(bytes, length);

            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Reads a <see cref="DateTime"/> from the stream using little-endian.
        /// </summary>
        public string ReadString()
            => ReadString(Endian.Little);

        /// <summary>
        /// Writes a <see cref="string"/> to the stream using the given endianness.
        /// </summary>
        public void WriteString(string value, Endian endian)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            int length = bytes.Length;

            WriteInt(length, endian);
            WriteCore(bytes, length);
        }

        /// <summary>
        /// Writes a <see cref="string"/> to the stream using little-endian.
        /// </summary>
        public void WriteString(string value)
            => WriteString(value, Endian.Little);

        private byte[] ReadCore(int count)
            => ReadCore(buffer, count);

        private byte[] ReadCore(byte[] buffer, int count)
        {
            if (stream.Read(buffer, 0, count) != count)
            {
                throw Ex.EndOfStream();
            }

            return buffer;
        }

        private void WriteCore(ReadOnlySpan<byte> buffer, int count)
        {
            stream.Write(buffer.ToArray(), 0, count);
        }
    }
}
