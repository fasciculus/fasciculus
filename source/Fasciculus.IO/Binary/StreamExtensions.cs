using System;
using System.IO;
using System.Text;

namespace Fasciculus.IO.Binary
{
    /// <summary>
    /// Extensions to read/write binary data from streams.
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Reads a <see cref="bool"/> from the stream
        /// </summary>
        public static bool ReadBool(this Stream stream)
            => stream.ReadByte() != 0;

        /// <summary>
        /// Writes a <see cref="bool"/> to the stream.
        /// </summary>
        public static void WriteBool(this Stream stream, bool value)
            => stream.WriteByte((byte)(value ? 1 : 0));

        /// <summary>
        /// Reads a <see cref="byte"/> from the stream
        /// </summary>
        public static byte ReadByte(this Stream stream)
            => ReadCore(stream, Alloc(1), 1)[0];

        /// <summary>
        /// Writes a <see cref="byte"/> to the stream.
        /// </summary>
        public static void WriteByte(this Stream stream, byte value)
            => WriteCore(stream, [value], 1);

        /// <summary>
        /// Reads a <see cref="sbyte"/> from the stream
        /// </summary>
        public static sbyte ReadSByte(this Stream stream)
            => (sbyte)stream.ReadByte();

        /// <summary>
        /// Writes a <see cref="sbyte"/> to the stream.
        /// </summary>
        public static void WriteSByte(this Stream stream, sbyte value)
            => stream.WriteByte((byte)value);

        /// <summary>
        /// Reads a <see cref="short"/> from the stream using the given endianness.
        /// </summary>
        public static short ReadShort(this Stream stream, Endian endian)
            => endian.GetShort(ReadCore(stream, Alloc(sizeof(short)), sizeof(short)));

        /// <summary>
        /// Reads a <see cref="short"/> from the stream using little-endian.
        /// </summary>
        public static short ReadShort(this Stream stream)
            => stream.ReadShort(Endian.Little);

        /// <summary>
        /// Writes a <see cref="short"/> to the stream using the given endianness.
        /// </summary>
        public static void WriteShort(this Stream stream, short value, Endian endian)
            => WriteCore(stream, endian.SetShort(Alloc(sizeof(short)), value), sizeof(short));

        /// <summary>
        /// Writes a <see cref="short"/> to the stream using little-endian.
        /// </summary>
        public static void WriteShort(this Stream stream, short value)
            => stream.WriteShort(value, Endian.Little);

        /// <summary>
        /// Reads a <see cref="ushort"/> from the stream using the given endianness.
        /// </summary>
        public static ushort ReadUShort(this Stream stream, Endian endian)
            => endian.GetUShort(ReadCore(stream, Alloc(sizeof(ushort)), sizeof(ushort)));

        /// <summary>
        /// Reads a <see cref="ushort"/> from the stream using little-endian.
        /// </summary>
        public static ushort ReadUShort(this Stream stream)
            => stream.ReadUShort(Endian.Little);

        /// <summary>
        /// Writes a <see cref="ushort"/> to the stream using the given endianness.
        /// </summary>
        public static void WriteUShort(this Stream stream, ushort value, Endian endian)
            => WriteCore(stream, endian.SetUShort(Alloc(sizeof(ushort)), value), sizeof(ushort));

        /// <summary>
        /// Writes a <see cref="ushort"/> to the stream using little-endian.
        /// </summary>
        public static void WriteUShort(this Stream stream, ushort value)
            => stream.WriteUShort(value, Endian.Little);

        /// <summary>
        /// Reads a <see cref="int"/> from the stream using the given endianness.
        /// </summary>
        public static int ReadInt(this Stream stream, Endian endian)
            => endian.GetInt(ReadCore(stream, Alloc(sizeof(int)), sizeof(int)));

        /// <summary>
        /// Reads a <see cref="int"/> from the stream using little-endian.
        /// </summary>
        public static int ReadInt(this Stream stream)
            => stream.ReadInt(Endian.Little);

        /// <summary>
        /// Writes a <see cref="int"/> to the stream using the given endianness.
        /// </summary>
        public static void WriteInt(this Stream stream, int value, Endian endian)
            => WriteCore(stream, endian.SetInt(Alloc(sizeof(int)), value), sizeof(int));

        /// <summary>
        /// Writes a <see cref="int"/> to the stream using little-endian.
        /// </summary>
        public static void WriteInt(this Stream stream, int value)
            => stream.WriteInt(value, Endian.Little);

        /// <summary>
        /// Reads a <see cref="uint"/> from the stream using the given endianness.
        /// </summary>
        public static uint ReadUInt(this Stream stream, Endian endian)
            => endian.GetUInt(ReadCore(stream, Alloc(sizeof(uint)), sizeof(uint)));

        /// <summary>
        /// Reads a <see cref="uint"/> from the stream using little-endian.
        /// </summary>
        public static uint ReadUInt(this Stream stream)
            => stream.ReadUInt(Endian.Little);

        /// <summary>
        /// Writes a <see cref="uint"/> to the stream using the given endianness.
        /// </summary>
        public static void WriteUInt(this Stream stream, uint value, Endian endian)
            => WriteCore(stream, endian.SetUInt(Alloc(sizeof(uint)), value), sizeof(uint));

        /// <summary>
        /// Writes a <see cref="uint"/> to the stream using little-endian.
        /// </summary>
        public static void WriteUInt(this Stream stream, uint value)
            => stream.WriteUInt(value, Endian.Little);

        /// <summary>
        /// Reads a <see cref="long"/> from the stream using the given endianness.
        /// </summary>
        public static long ReadLong(this Stream stream, Endian endian)
            => endian.GetLong(ReadCore(stream, Alloc(sizeof(long)), sizeof(long)));

        /// <summary>
        /// Reads a <see cref="long"/> from the stream using little-endian.
        /// </summary>
        public static long ReadLong(this Stream stream)
            => stream.ReadLong(Endian.Little);

        /// <summary>
        /// Writes a <see cref="long"/> to the stream using the given endianness.
        /// </summary>
        public static void WriteLong(this Stream stream, long value, Endian endian)
            => WriteCore(stream, endian.SetLong(Alloc(sizeof(long)), value), sizeof(long));

        /// <summary>
        /// Writes a <see cref="long"/> to the stream using little-endian.
        /// </summary>
        public static void WriteLong(this Stream stream, long value)
            => stream.WriteLong(value, Endian.Little);

        /// <summary>
        /// Reads a <see cref="ulong"/> from the stream using the given endianness.
        /// </summary>
        public static ulong ReadULong(this Stream stream, Endian endian)
            => endian.GetULong(ReadCore(stream, Alloc(sizeof(ulong)), sizeof(ulong)));

        /// <summary>
        /// Reads a <see cref="ulong"/> from the stream using little-endian.
        /// </summary>
        public static ulong ReadULong(this Stream stream)
            => stream.ReadULong(Endian.Little);

        /// <summary>
        /// Writes a <see cref="ulong"/> to the stream using the given endianness.
        /// </summary>
        public static void WriteULong(this Stream stream, ulong value, Endian endian)
            => WriteCore(stream, endian.SetULong(Alloc(sizeof(ulong)), value), sizeof(ulong));

        /// <summary>
        /// Writes a <see cref="ulong"/> to the stream using little-endian.
        /// </summary>
        public static void WriteULong(this Stream stream, ulong value)
            => stream.WriteULong(value, Endian.Little);

        /// <summary>
        /// Reads a <see cref="float"/> from the stream using the given endianness.
        /// </summary>
        public static float ReadFloat(this Stream stream, Endian endian)
            => endian.GetFloat(ReadCore(stream, Alloc(sizeof(float)), sizeof(float)));

        /// <summary>
        /// Reads a <see cref="float"/> from the stream using little-endian.
        /// </summary>
        public static float ReadFloat(this Stream stream)
            => stream.ReadFloat(Endian.Little);

        /// <summary>
        /// Writes a <see cref="float"/> to the stream using the given endianness.
        /// </summary>
        public static void WriteFloat(this Stream stream, float value, Endian endian)
            => WriteCore(stream, endian.SetFloat(Alloc(sizeof(float)), value), sizeof(float));

        /// <summary>
        /// Writes a <see cref="float"/> to the stream using little-endian.
        /// </summary>
        public static void WriteFloat(this Stream stream, float value)
            => stream.WriteFloat(value, Endian.Little);

        /// <summary>
        /// Reads a <see cref="double"/> from the stream using the given endianness.
        /// </summary>
        public static double ReadDouble(this Stream stream, Endian endian)
            => endian.GetDouble(ReadCore(stream, Alloc(sizeof(double)), sizeof(double)));

        /// <summary>
        /// Reads a <see cref="double"/> from the stream using little-endian.
        /// </summary>
        public static double ReadDouble(this Stream stream)
            => stream.ReadDouble(Endian.Little);

        /// <summary>
        /// Writes a <see cref="double"/> to the stream using the given endianness.
        /// </summary>
        public static void WriteDouble(this Stream stream, double value, Endian endian)
            => WriteCore(stream, endian.SetDouble(Alloc(sizeof(double)), value), sizeof(double));

        /// <summary>
        /// Writes a <see cref="double"/> to the stream using little-endian.
        /// </summary>
        public static void WriteDouble(this Stream stream, double value)
            => stream.WriteDouble(value, Endian.Little);

        /// <summary>
        /// Reads a <see cref="DateTime"/> from the stream using the given endianness.
        /// </summary>
        public static DateTime ReadDateTime(this Stream stream, Endian endian)
            => DateTime.FromBinary(stream.ReadLong(endian));

        /// <summary>
        /// Reads a <see cref="DateTime"/> from the stream using little-endian.
        /// </summary>
        public static DateTime ReadDateTime(this Stream stream)
            => stream.ReadDateTime(Endian.Little);

        /// <summary>
        /// Writes a <see cref="DateTime"/> to the stream using the given endianness.
        /// </summary>
        public static void WriteDateTime(this Stream stream, DateTime value, Endian endian)
            => stream.WriteLong(value.ToBinary(), endian);

        /// <summary>
        /// Writes a <see cref="DateTime"/> to the stream using little-endian.
        /// </summary>
        public static void WriteDateTime(this Stream stream, DateTime value)
            => stream.WriteDateTime(value, Endian.Little);

        /// <summary>
        /// Reads a <see cref="string"/> from the stream using the given endianness.
        /// </summary>
        public static string ReadString(this Stream stream, Endian endian)
        {
            int count = stream.ReadInt(endian);
            byte[] buffer = Alloc(count);

            ReadCore(stream, buffer, count);

            return Encoding.UTF8.GetString(buffer);
        }

        /// <summary>
        /// Reads a <see cref="DateTime"/> from the stream using little-endian.
        /// </summary>
        public static string ReadString(this Stream stream)
            => stream.ReadString(Endian.Little);

        /// <summary>
        /// Writes a <see cref="string"/> to the stream using the given endianness.
        /// </summary>
        public static void WriteString(this Stream stream, string value, Endian endian)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(value);

            if (buffer.LongLength > int.MaxValue)
            {
                throw new IndexOutOfRangeException();
            }

            int count = buffer.Length;

            stream.WriteInt(buffer.Length, endian);
            WriteCore(stream, buffer, count);
        }

        /// <summary>
        /// Writes a <see cref="string"/> to the stream using little-endian.
        /// </summary>
        public static void WriteString(this Stream stream, string value)
            => stream.WriteString(value, Endian.Little);

        private static byte[] Alloc(int count)
            => new byte[count];

        private static byte[] ReadCore(Stream stream, byte[] buffer, int count)
        {
            if (stream.Read(buffer, 0, count) != count)
            {
                throw new EndOfStreamException();
            }

            return buffer;
        }

        private static void WriteCore(Stream stream, byte[] buffer, int count)
            => stream.Write(buffer, 0, count);

        private static void WriteCore(Stream stream, ReadOnlySpan<byte> buffer, int count)
            => WriteCore(stream, buffer.ToArray(), count);
    }
}
