using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            => stream.ReadUInt8() != 0;

        /// <summary>
        /// Writes a <see cref="bool"/> to the stream.
        /// </summary>
        public static void WriteBool(this Stream stream, bool value)
            => stream.WriteUInt8((byte)(value ? 1 : 0));

        /// <summary>
        /// Reads a <see cref="sbyte"/> from the stream
        /// </summary>
        public static sbyte ReadInt8(this Stream stream)
            => (sbyte)stream.ReadUInt8();

        /// <summary>
        /// Writes a <see cref="sbyte"/> to the stream.
        /// </summary>
        public static void WriteInt8(this Stream stream, sbyte value)
            => stream.WriteUInt8((byte)value);

        /// <summary>
        /// Reads a <see cref="byte"/> from the stream
        /// </summary>
        public static byte ReadUInt8(this Stream stream)
            => ReadCore(stream, Alloc(1), 1)[0];

        /// <summary>
        /// Writes a <see cref="byte"/> to the stream.
        /// </summary>
        public static void WriteUInt8(this Stream stream, byte value)
            => WriteCore(stream, [value], 1);

        /// <summary>
        /// Reads a <see cref="short"/> from the stream using the given endianness.
        /// </summary>
        public static short ReadInt16(this Stream stream, Endian endian)
            => endian.GetShort(ReadCore(stream, Alloc(sizeof(short)), sizeof(short)));

        /// <summary>
        /// Reads a <see cref="short"/> from the stream using little-endian.
        /// </summary>
        public static short ReadInt16(this Stream stream)
            => stream.ReadInt16(Endian.Little);

        /// <summary>
        /// Writes a <see cref="short"/> to the stream using the given endianness.
        /// </summary>
        public static void WriteInt16(this Stream stream, short value, Endian endian)
            => WriteCore(stream, endian.SetShort(Alloc(sizeof(short)), value), sizeof(short));

        /// <summary>
        /// Writes a <see cref="short"/> to the stream using little-endian.
        /// </summary>
        public static void WriteInt16(this Stream stream, short value)
            => stream.WriteInt16(value, Endian.Little);

        /// <summary>
        /// Reads a <see cref="ushort"/> from the stream using the given endianness.
        /// </summary>
        public static ushort ReadUInt16(this Stream stream, Endian endian)
            => endian.GetUShort(ReadCore(stream, Alloc(sizeof(ushort)), sizeof(ushort)));

        /// <summary>
        /// Reads a <see cref="ushort"/> from the stream using little-endian.
        /// </summary>
        public static ushort ReadUInt16(this Stream stream)
            => stream.ReadUInt16(Endian.Little);

        /// <summary>
        /// Writes a <see cref="ushort"/> to the stream using the given endianness.
        /// </summary>
        public static void WriteUInt16(this Stream stream, ushort value, Endian endian)
            => WriteCore(stream, endian.SetUShort(Alloc(sizeof(ushort)), value), sizeof(ushort));

        /// <summary>
        /// Writes a <see cref="ushort"/> to the stream using little-endian.
        /// </summary>
        public static void WriteUInt16(this Stream stream, ushort value)
            => stream.WriteUInt16(value, Endian.Little);

        /// <summary>
        /// Reads a <see cref="int"/> from the stream using the given endianness.
        /// </summary>
        public static int ReadInt32(this Stream stream, Endian endian)
            => endian.GetInt(ReadCore(stream, Alloc(sizeof(int)), sizeof(int)));

        /// <summary>
        /// Reads a <see cref="int"/> from the stream using little-endian.
        /// </summary>
        public static int ReadInt32(this Stream stream)
            => stream.ReadInt32(Endian.Little);

        /// <summary>
        /// Writes a <see cref="int"/> to the stream using the given endianness.
        /// </summary>
        public static void WriteInt32(this Stream stream, int value, Endian endian)
            => WriteCore(stream, endian.SetInt(Alloc(sizeof(int)), value), sizeof(int));

        /// <summary>
        /// Writes a <see cref="int"/> to the stream using little-endian.
        /// </summary>
        public static void WriteInt32(this Stream stream, int value)
            => stream.WriteInt32(value, Endian.Little);

        /// <summary>
        /// Reads a <see cref="uint"/> from the stream using the given endianness.
        /// </summary>
        public static uint ReadUInt32(this Stream stream, Endian endian)
            => endian.GetUInt(ReadCore(stream, Alloc(sizeof(uint)), sizeof(uint)));

        /// <summary>
        /// Reads a <see cref="uint"/> from the stream using little-endian.
        /// </summary>
        public static uint ReadUInt32(this Stream stream)
            => stream.ReadUInt32(Endian.Little);

        /// <summary>
        /// Writes a <see cref="uint"/> to the stream using the given endianness.
        /// </summary>
        public static void WriteUInt32(this Stream stream, uint value, Endian endian)
            => WriteCore(stream, endian.SetUInt(Alloc(sizeof(uint)), value), sizeof(uint));

        /// <summary>
        /// Writes a <see cref="uint"/> to the stream using little-endian.
        /// </summary>
        public static void WriteUInt32(this Stream stream, uint value)
            => stream.WriteUInt32(value, Endian.Little);

        /// <summary>
        /// Reads a <see cref="long"/> from the stream using the given endianness.
        /// </summary>
        public static long ReadInt64(this Stream stream, Endian endian)
            => endian.GetLong(ReadCore(stream, Alloc(sizeof(long)), sizeof(long)));

        /// <summary>
        /// Reads a <see cref="long"/> from the stream using little-endian.
        /// </summary>
        public static long ReadInt64(this Stream stream)
            => stream.ReadInt64(Endian.Little);

        /// <summary>
        /// Writes a <see cref="long"/> to the stream using the given endianness.
        /// </summary>
        public static void WriteInt64(this Stream stream, long value, Endian endian)
            => WriteCore(stream, endian.SetLong(Alloc(sizeof(long)), value), sizeof(long));

        /// <summary>
        /// Writes a <see cref="long"/> to the stream using little-endian.
        /// </summary>
        public static void WriteInt64(this Stream stream, long value)
            => stream.WriteInt64(value, Endian.Little);

        /// <summary>
        /// Reads a <see cref="ulong"/> from the stream using the given endianness.
        /// </summary>
        public static ulong ReadUInt64(this Stream stream, Endian endian)
            => endian.GetULong(ReadCore(stream, Alloc(sizeof(ulong)), sizeof(ulong)));

        /// <summary>
        /// Reads a <see cref="ulong"/> from the stream using little-endian.
        /// </summary>
        public static ulong ReadUInt64(this Stream stream)
            => stream.ReadUInt64(Endian.Little);

        /// <summary>
        /// Writes a <see cref="ulong"/> to the stream using the given endianness.
        /// </summary>
        public static void WriteUInt64(this Stream stream, ulong value, Endian endian)
            => WriteCore(stream, endian.SetULong(Alloc(sizeof(ulong)), value), sizeof(ulong));

        /// <summary>
        /// Writes a <see cref="ulong"/> to the stream using little-endian.
        /// </summary>
        public static void WriteUInt64(this Stream stream, ulong value)
            => stream.WriteUInt64(value, Endian.Little);

        /// <summary>
        /// Reads a <see cref="float"/> from the stream using the given endianness.
        /// </summary>
        public static float ReadSingle(this Stream stream, Endian endian)
            => endian.GetFloat(ReadCore(stream, Alloc(sizeof(float)), sizeof(float)));

        /// <summary>
        /// Reads a <see cref="float"/> from the stream using little-endian.
        /// </summary>
        public static float ReadSingle(this Stream stream)
            => stream.ReadSingle(Endian.Little);

        /// <summary>
        /// Writes a <see cref="float"/> to the stream using the given endianness.
        /// </summary>
        public static void WriteSingle(this Stream stream, float value, Endian endian)
            => WriteCore(stream, endian.SetFloat(Alloc(sizeof(float)), value), sizeof(float));

        /// <summary>
        /// Writes a <see cref="float"/> to the stream using little-endian.
        /// </summary>
        public static void WriteSingle(this Stream stream, float value)
            => stream.WriteSingle(value, Endian.Little);

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
            => DateTime.FromBinary(stream.ReadInt64(endian));

        /// <summary>
        /// Reads a <see cref="DateTime"/> from the stream using little-endian.
        /// </summary>
        public static DateTime ReadDateTime(this Stream stream)
            => stream.ReadDateTime(Endian.Little);

        /// <summary>
        /// Writes a <see cref="DateTime"/> to the stream using the given endianness.
        /// </summary>
        public static void WriteDateTime(this Stream stream, DateTime value, Endian endian)
            => stream.WriteInt64(value.ToBinary(), endian);

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
            int count = stream.ReadInt32(endian);
            byte[] buffer = Alloc(count);

            ReadCore(stream, buffer, count);

            return Encoding.UTF8.GetString(buffer);
        }

        /// <summary>
        /// Reads a <see cref="string"/> from the stream using little-endian.
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
                throw new ArgumentOutOfRangeException();
            }

            int count = buffer.Length;

            stream.WriteInt32(buffer.Length, endian);
            WriteCore(stream, buffer, count);
        }

        /// <summary>
        /// Writes a <see cref="string"/> to the stream using little-endian.
        /// </summary>
        public static void WriteString(this Stream stream, string value)
            => stream.WriteString(value, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="bool"/> using the given <paramref name="endian"/>.
        /// </summary>
        public static bool[] ReadBoolArray(this Stream stream, Endian endian)
            => ReadArrayCore(stream, endian, (s, e) => s.ReadBool());

        /// <summary>
        /// Reads an array of <see cref="bool"/> using little-endian.
        /// </summary>
        public static bool[] ReadBoolArray(this Stream stream)
            => stream.ReadBoolArray(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="bool"/> using the given <paramref name="endian"/>.
        /// </summary>
        public static void WriteBoolArray(this Stream stream, bool[] values, Endian endian)
            => WriteArrayCore(stream, endian, values, (s, t, e) => s.WriteBool(t));

        /// <summary>
        /// Writes an array of <see cref="bool"/> using little-endian.
        /// </summary>
        public static void WriteBoolArray(this Stream stream, bool[] values)
            => stream.WriteBoolArray(values, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="sbyte"/> using the given <paramref name="endian"/>.
        /// </summary>
        public static sbyte[] ReadInt8Array(this Stream stream, Endian endian)
            => ReadArrayCore(stream, endian, (s, e) => s.ReadInt8());

        /// <summary>
        /// Reads an array of <see cref="sbyte"/> using little-endian..
        /// </summary>
        public static sbyte[] ReadInt8Array(this Stream stream)
            => stream.ReadInt8Array(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="sbyte"/> using the given <paramref name="endian"/>.
        /// </summary>
        public static void WriteInt8Array(this Stream stream, sbyte[] values, Endian endian)
            => WriteArrayCore(stream, endian, values, (s, v, e) => s.WriteInt8(v));

        /// <summary>
        /// Writes an array of <see cref="sbyte"/> using little-endian.
        /// </summary>
        public static void WriteInt8Array(this Stream stream, sbyte[] values)
            => stream.WriteInt8Array(values, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="byte"/> using the given <paramref name="endian"/>.
        /// </summary>
        public static byte[] ReadUInt8Array(this Stream stream, Endian endian)
            => ReadArrayCore(stream, endian, (s, e) => s.ReadUInt8());

        /// <summary>
        /// Reads an array of <see cref="byte"/> using little-endian..
        /// </summary>
        public static byte[] ReadUInt8Array(this Stream stream)
            => stream.ReadUInt8Array(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="byte"/> using the given <paramref name="endian"/>.
        /// </summary>
        public static void WriteUInt8Array(this Stream stream, byte[] values, Endian endian)
            => WriteArrayCore(stream, endian, values, (s, v, e) => s.WriteUInt8(v));

        /// <summary>
        /// Writes an array of <see cref="byte"/> using little-endian.
        /// </summary>
        public static void WriteUInt8Array(this Stream stream, byte[] values)
            => stream.WriteUInt8Array(values, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="short"/> using the given <paramref name="endian"/>.
        /// </summary>
        public static short[] ReadInt16Array(this Stream stream, Endian endian)
            => ReadArrayCore(stream, endian, (s, e) => s.ReadInt16(e));

        /// <summary>
        /// Reads an array of <see cref="short"/> using little-endian..
        /// </summary>
        public static short[] ReadInt16Array(this Stream stream)
            => stream.ReadInt16Array(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="short"/> using the given <paramref name="endian"/>.
        /// </summary>
        public static void WriteInt16Array(this Stream stream, short[] values, Endian endian)
            => WriteArrayCore(stream, endian, values, (s, v, e) => s.WriteInt16(v, e));

        /// <summary>
        /// Writes an array of <see cref="short"/> using little-endian.
        /// </summary>
        public static void WriteInt16Array(this Stream stream, short[] values)
            => stream.WriteInt16Array(values, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="ushort"/> using the given <paramref name="endian"/>.
        /// </summary>
        public static ushort[] ReadUInt16Array(this Stream stream, Endian endian)
            => ReadArrayCore(stream, endian, (s, e) => s.ReadUInt16(e));

        /// <summary>
        /// Reads an array of <see cref="ushort"/> using little-endian..
        /// </summary>
        public static ushort[] ReadUInt16Array(this Stream stream)
            => stream.ReadUInt16Array(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="ushort"/> using the given <paramref name="endian"/>.
        /// </summary>
        public static void WriteUInt16Array(this Stream stream, ushort[] values, Endian endian)
            => WriteArrayCore(stream, endian, values, (s, v, e) => s.WriteUInt16(v, e));

        /// <summary>
        /// Writes an array of <see cref="ushort"/> using little-endian.
        /// </summary>
        public static void WriteUInt16Array(this Stream stream, ushort[] values)
            => stream.WriteUInt16Array(values, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="int"/> using the given <paramref name="endian"/>.
        /// </summary>
        public static int[] ReadInt32Array(this Stream stream, Endian endian)
            => ReadArrayCore(stream, endian, (s, e) => s.ReadInt32(e));

        /// <summary>
        /// Reads an array of <see cref="int"/> using little-endian..
        /// </summary>
        public static int[] ReadInt32Array(this Stream stream)
            => stream.ReadInt32Array(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="int"/> using the given <paramref name="endian"/>.
        /// </summary>
        public static void WriteInt32Array(this Stream stream, int[] values, Endian endian)
            => WriteArrayCore(stream, endian, values, (s, v, e) => s.WriteInt32(v, e));

        /// <summary>
        /// Writes an array of <see cref="int"/> using little-endian.
        /// </summary>
        public static void WriteInt32Array(this Stream stream, int[] values)
            => stream.WriteInt32Array(values, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="uint"/> using the given <paramref name="endian"/>.
        /// </summary>
        public static uint[] ReadUInt32Array(this Stream stream, Endian endian)
            => ReadArrayCore(stream, endian, (s, e) => s.ReadUInt32(e));

        /// <summary>
        /// Reads an array of <see cref="uint"/> using little-endian..
        /// </summary>
        public static uint[] ReadUInt32Array(this Stream stream)
            => stream.ReadUInt32Array(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="uint"/> using the given <paramref name="endian"/>.
        /// </summary>
        public static void WriteUInt32Array(this Stream stream, uint[] values, Endian endian)
            => WriteArrayCore(stream, endian, values, (s, v, e) => s.WriteUInt32(v, e));

        /// <summary>
        /// Writes an array of <see cref="uint"/> using little-endian.
        /// </summary>
        public static void WriteUInt32Array(this Stream stream, uint[] values)
            => stream.WriteUInt32Array(values, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="long"/> using the given <paramref name="endian"/>.
        /// </summary>
        public static long[] ReadInt64Array(this Stream stream, Endian endian)
            => ReadArrayCore(stream, endian, (s, e) => s.ReadInt64(e));

        /// <summary>
        /// Reads an array of <see cref="long"/> using little-endian..
        /// </summary>
        public static long[] ReadInt64Array(this Stream stream)
            => stream.ReadInt64Array(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="long"/> using the given <paramref name="endian"/>.
        /// </summary>
        public static void WriteInt64Array(this Stream stream, long[] values, Endian endian)
            => WriteArrayCore(stream, endian, values, (s, v, e) => s.WriteInt64(v, e));

        /// <summary>
        /// Writes an array of <see cref="long"/> using little-endian.
        /// </summary>
        public static void WriteInt64Array(this Stream stream, long[] values)
            => stream.WriteInt64Array(values, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="ulong"/> using the given <paramref name="endian"/>.
        /// </summary>
        public static ulong[] ReadUInt64Array(this Stream stream, Endian endian)
            => ReadArrayCore(stream, endian, (s, e) => s.ReadUInt64(e));

        /// <summary>
        /// Reads an array of <see cref="ulong"/> using little-endian..
        /// </summary>
        public static ulong[] ReadUInt64Array(this Stream stream)
            => stream.ReadUInt64Array(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="ulong"/> using the given <paramref name="endian"/>.
        /// </summary>
        public static void WriteUInt64Array(this Stream stream, ulong[] values, Endian endian)
            => WriteArrayCore(stream, endian, values, (s, v, e) => s.WriteUInt64(v, e));

        /// <summary>
        /// Writes an array of <see cref="ulong"/> using little-endian.
        /// </summary>
        public static void WriteUInt64Array(this Stream stream, ulong[] values)
            => stream.WriteUInt64Array(values, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="float"/> using the given <paramref name="endian"/>.
        /// </summary>
        public static float[] ReadSingleArray(this Stream stream, Endian endian)
            => ReadArrayCore(stream, endian, (s, e) => s.ReadSingle(e));

        /// <summary>
        /// Reads an array of <see cref="float"/> using little-endian..
        /// </summary>
        public static float[] ReadSingleArray(this Stream stream)
            => stream.ReadSingleArray(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="float"/> using the given <paramref name="endian"/>.
        /// </summary>
        public static void WriteSingleArray(this Stream stream, float[] values, Endian endian)
            => WriteArrayCore(stream, endian, values, (s, v, e) => s.WriteSingle(v, e));

        /// <summary>
        /// Writes an array of <see cref="float"/> using little-endian.
        /// </summary>
        public static void WriteSingleArray(this Stream stream, float[] values)
            => stream.WriteSingleArray(values, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="double"/> using the given <paramref name="endian"/>.
        /// </summary>
        public static double[] ReadDoubleArray(this Stream stream, Endian endian)
            => ReadArrayCore(stream, endian, (s, e) => s.ReadDouble(e));

        /// <summary>
        /// Reads an array of <see cref="double"/> using little-endian..
        /// </summary>
        public static double[] ReadDoubleArray(this Stream stream)
            => stream.ReadDoubleArray(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="double"/> using the given <paramref name="endian"/>.
        /// </summary>
        public static void WriteDoubleArray(this Stream stream, double[] values, Endian endian)
            => WriteArrayCore(stream, endian, values, (s, v, e) => s.WriteDouble(v, e));

        /// <summary>
        /// Writes an array of <see cref="double"/> using little-endian.
        /// </summary>
        public static void WriteDoubleArray(this Stream stream, double[] values)
            => stream.WriteDoubleArray(values, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="DateTime"/> using the given <paramref name="endian"/>.
        /// </summary>
        public static DateTime[] ReadDateTimeArray(this Stream stream, Endian endian)
            => ReadArrayCore(stream, endian, (s, e) => s.ReadDateTime(e));

        /// <summary>
        /// Reads an array of <see cref="DateTime"/> using little-endian..
        /// </summary>
        public static DateTime[] ReadDateTimeArray(this Stream stream)
            => stream.ReadDateTimeArray(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="DateTime"/> using the given <paramref name="endian"/>.
        /// </summary>
        public static void WriteDateTimeArray(this Stream stream, DateTime[] values, Endian endian)
            => WriteArrayCore(stream, endian, values, (s, v, e) => s.WriteDateTime(v, e));

        /// <summary>
        /// Writes an array of <see cref="DateTime"/> using little-endian.
        /// </summary>
        public static void WriteDateTimeArray(this Stream stream, DateTime[] values)
            => stream.WriteDateTimeArray(values, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="string"/> using the given <paramref name="endian"/>.
        /// </summary>
        public static string[] ReadStringArray(this Stream stream, Endian endian)
            => ReadArrayCore(stream, endian, (s, e) => s.ReadString(e));

        /// <summary>
        /// Reads an array of <see cref="string"/> using little-endian..
        /// </summary>
        public static string[] ReadStringArray(this Stream stream)
            => stream.ReadStringArray(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="string"/> using the given <paramref name="endian"/>.
        /// </summary>
        public static void WriteStringArray(this Stream stream, string[] values, Endian endian)
            => WriteArrayCore(stream, endian, values, (s, v, e) => s.WriteString(v, e));

        /// <summary>
        /// Writes an array of <see cref="string"/> using little-endian.
        /// </summary>
        public static void WriteStringArray(this Stream stream, string[] values)
            => stream.WriteStringArray(values, Endian.Little);

        /// <summary>
        /// Reads an array of type <typeparamref name="T"/> with the given <paramref name="endian"/>.
        /// </summary>
        public static T[] ReadArray<T>(this Stream stream, Func<Stream, Endian, T> read, Endian endian)
            => ReadArrayCore(stream, endian, read);

        /// <summary>
        /// Reads an array of type <typeparamref name="T"/> using little-endian.
        /// </summary>
        public static T[] ReadArray<T>(this Stream stream, Func<Stream, Endian, T> read)
            => stream.ReadArray(read, Endian.Little);

        /// <summary>
        /// Reads an array of type <typeparamref name="T"/> using little-endian.
        /// </summary>
        public static T[] ReadArray<T>(this Stream stream, Func<Stream, T> read)
            => stream.ReadArray((s, e) => read(s), Endian.Little);

        /// <summary>
        /// Writes an array of type <typeparamref name="T"/> with the given <paramref name="endian"/>.
        /// </summary>
        public static void WriteArray<T>(this Stream stream, T[] values, Action<Stream, T, Endian> write, Endian endian)
            => WriteArrayCore(stream, endian, values, write);

        /// <summary>
        /// Writes an array of type <typeparamref name="T"/> using little-endian.
        /// </summary>
        public static void WriteArray<T>(this Stream stream, T[] values, Action<Stream, T, Endian> write)
            => stream.WriteArray(values, write, Endian.Little);

        /// <summary>
        /// Writes an array of type <typeparamref name="T"/> using little-endian.
        /// </summary>
        public static void WriteArray<T>(this Stream stream, T[] values, Action<Stream, T> write)
            => stream.WriteArray(values, (s, v, e) => { write(s, v); }, Endian.Little);

        /// <summary>
        /// Reads a dictionary using the given <paramref name="endian"/>.
        /// </summary>
        public static Dictionary<K, V> ReadDictionary<K, V>(this Stream stream, Func<Stream, Endian, K> readKey,
            Func<Stream, Endian, V> readValue, Endian endian)
            where K : notnull
        {
            int count = stream.ReadInt32(endian);
            Dictionary<K, V> dictionary = new(count);

            for (int i = 0; i < count; ++i)
            {
                K key = readKey(stream, endian);
                V value = readValue(stream, endian);

                dictionary.Add(key, value);
            }

            return dictionary;
        }

        /// <summary>
        /// Reads a dictionary using little-endian.
        /// </summary>
        public static Dictionary<K, V> ReadDictionary<K, V>(this Stream stream, Func<Stream, Endian, K> readKey,
            Func<Stream, Endian, V> readValue)
            where K : notnull
            => stream.ReadDictionary(readKey, readValue, Endian.Little);

        /// <summary>
        /// Reads a dictionary using little-endian.
        /// </summary>
        public static Dictionary<K, V> ReadDictionary<K, V>(this Stream stream, Func<Stream, K> readKey, Func<Stream, V> readValue)
            where K : notnull
            => stream.ReadDictionary((s, e) => readKey(s), (s, e) => readValue(s), Endian.Little);

        /// <summary>
        /// Writes a dictionary using the given <paramref name="endian"/>.
        /// </summary>
        /// <remarks>
        /// The <typeparamref name="K"/> type must implement <see cref="IComparable{K}"/> to get deterministic results.
        /// </remarks>
        public static void WriteDictionary<K, V>(this Stream stream, IReadOnlyDictionary<K, V> dictionary,
            Action<Stream, K, Endian> writeKey, Action<Stream, V, Endian> writeValue, Endian endian)
            where K : notnull
        {
            stream.WriteInt32(dictionary.Count);

            foreach (KeyValuePair<K, V> kvp in dictionary.OrderBy(x => x.Key))
            {
                writeKey(stream, kvp.Key, endian);
                writeValue(stream, kvp.Value, endian);
            }
        }

        /// <summary>
        /// Writes a dictionary using little-endian.
        /// </summary>
        /// <remarks>
        /// The <typeparamref name="K"/> type must implement <see cref="IComparable{K}"/> to get deterministic results.
        /// </remarks>
        public static void WriteDictionary<K, V>(this Stream stream, IReadOnlyDictionary<K, V> dictionary,
            Action<Stream, K, Endian> writeKey, Action<Stream, V, Endian> writeValue)
            where K : notnull
            => stream.WriteDictionary(dictionary, writeKey, writeValue, Endian.Little);

        /// <summary>
        /// Writes a dictionary using little-endian.
        /// </summary>
        /// <remarks>
        /// The <typeparamref name="K"/> type must implement <see cref="IComparable{K}"/> to get deterministic results.
        /// </remarks>
        public static void WriteDictionary<K, V>(this Stream stream, IReadOnlyDictionary<K, V> dictionary,
            Action<Stream, K> writeKey, Action<Stream, V> writeValue)
            where K : notnull
            => stream.WriteDictionary(dictionary, (s, k, e) => { writeKey(s, k); }, (s, v, e) => { writeValue(s, v); }, Endian.Little);

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

        private static T[] ReadArrayCore<T>(Stream stream, Endian endian, Func<Stream, Endian, T> read)
        {
            int count = stream.ReadInt32(endian);

            return [.. Enumerable.Range(0, count).Select(_ => read(stream, endian))];
        }

        private static void WriteArrayCore<T>(Stream stream, Endian endian, T[] values, Action<Stream, T, Endian> write)
        {
            if (values.LongLength > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException();
            }

            int count = values.Length;

            stream.WriteInt32(count);

            for (int i = 0; i < count; ++i)
            {
                write(stream, values[i], endian);
            }
        }
    }
}
