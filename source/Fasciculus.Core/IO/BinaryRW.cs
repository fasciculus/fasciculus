using Fasciculus.Collections;
using Fasciculus.IO.Binary;
using Fasciculus.Support;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Fasciculus.IO
{
    /// <summary>
    /// Read and write binary data from or to a stream.
    /// </summary>
    public class BinaryRW
    {
        private readonly Stream stream;

        private readonly byte[] smallBuffer = new byte[16];

        /// <summary>
        /// Initializes a binary reader/writer with the given <paramref name="stream"/>
        /// </summary>
        public BinaryRW(Stream stream)
        {
            this.stream = stream;
        }

        /// <summary>
        /// Implicit creation of a <see cref="BinaryRW"/> from a <see cref="Stream"/>
        /// </summary>
        public static implicit operator BinaryRW(Stream stream)
            => new(stream);

        /// <summary>
        /// Reads a <see cref="bool"/> from the stream
        /// </summary>
        public bool ReadBool()
            => ReadUInt8() != 0;

        /// <summary>
        /// Writes a <see cref="bool"/> to the stream.
        /// </summary>
        public void WriteBool(bool value)
            => WriteUInt8((byte)(value ? 1 : 0));

        /// <summary>
        /// Reads a <see cref="byte"/> from the stream
        /// </summary>
        public byte ReadUInt8()
            => ReadCore(1)[0];

        /// <summary>
        /// Writes a <see cref="byte"/> to the stream.
        /// </summary>
        public void WriteUInt8(byte value)
            => WriteCore([value], 1);

        /// <summary>
        /// Reads a <see cref="sbyte"/> from the stream
        /// </summary>
        public sbyte ReadInt8()
            => (sbyte)ReadUInt8();

        /// <summary>
        /// Writes a <see cref="sbyte"/> to the stream.
        /// </summary>
        public void WriteInt8(sbyte value)
            => WriteUInt8((byte)value);

        /// <summary>
        /// Reads a <see cref="short"/> from the stream using the given endianness.
        /// </summary>
        public short ReadInt16(Endian endian)
            => endian.GetShort(ReadCore(sizeof(short)));

        /// <summary>
        /// Reads a <see cref="short"/> from the stream using little-endian.
        /// </summary>
        public short ReadInt16()
            => ReadInt16(Endian.Little);

        /// <summary>
        /// Writes a <see cref="short"/> to the stream using the given endianness.
        /// </summary>
        public void WriteInt16(short value, Endian endian)
            => WriteCore(endian.SetShort(smallBuffer, value), sizeof(short));

        /// <summary>
        /// Writes a <see cref="short"/> to the stream using little-endian.
        /// </summary>
        public void WriteInt16(short value)
            => WriteInt16(value, Endian.Little);

        /// <summary>
        /// Reads a <see cref="ushort"/> from the stream using the given endianness.
        /// </summary>
        public ushort ReadUInt16(Endian endian)
            => endian.GetUShort(ReadCore(sizeof(short)));

        /// <summary>
        /// Reads a <see cref="ushort"/> from the stream using little-endian.
        /// </summary>
        public ushort ReadUInt16()
            => ReadUInt16(Endian.Little);

        /// <summary>
        /// Writes a <see cref="ushort"/> to the stream using the given endianness.
        /// </summary>
        public void WriteUInt16(ushort value, Endian endian)
            => WriteCore(endian.SetUShort(smallBuffer, value), sizeof(short));

        /// <summary>
        /// Writes a <see cref="ushort"/> to the stream using little-endian.
        /// </summary>
        public void WriteUInt16(ushort value)
            => WriteUInt16(value, Endian.Little);

        /// <summary>
        /// Reads a <see cref="int"/> from the stream using the given endianness.
        /// </summary>
        public int ReadInt32(Endian endian)
            => endian.GetInt(ReadCore(sizeof(int)));

        /// <summary>
        /// Reads a <see cref="int"/> from the stream using little-endian.
        /// </summary>
        public int ReadInt32()
            => ReadInt32(Endian.Little);

        /// <summary>
        /// Writes a <see cref="int"/> to the stream using the given endianness.
        /// </summary>
        public void WriteInt32(int value, Endian endian)
            => WriteCore(endian.SetInt(smallBuffer, value), sizeof(int));

        /// <summary>
        /// Writes a <see cref="int"/> to the stream using little-endian.
        /// </summary>
        public void WriteInt32(int value)
            => WriteInt32(value, Endian.Little);

        /// <summary>
        /// Reads a <see cref="uint"/> from the stream using the given endianness.
        /// </summary>
        public uint ReadUInt32(Endian endian)
            => endian.GetUInt(ReadCore(sizeof(uint)));

        /// <summary>
        /// Reads a <see cref="uint"/> from the stream using little-endian.
        /// </summary>
        public uint ReadUInt32()
            => ReadUInt32(Endian.Little);

        /// <summary>
        /// Writes a <see cref="uint"/> to the stream using the given endianness.
        /// </summary>
        public void WriteUInt32(uint value, Endian endian)
            => WriteCore(endian.SetUInt(smallBuffer, value), sizeof(uint));

        /// <summary>
        /// Writes a <see cref="uint"/> to the stream using little-endian.
        /// </summary>
        public void WriteUInt32(uint value)
            => WriteUInt32(value, Endian.Little);

        /// <summary>
        /// Reads a <see cref="long"/> from the stream using the given endianness.
        /// </summary>
        public long ReadInt64(Endian endian)
            => endian.GetLong(ReadCore(sizeof(long)));

        /// <summary>
        /// Reads a <see cref="long"/> from the stream using little-endian.
        /// </summary>
        public long ReadInt64()
            => ReadInt64(Endian.Little);

        /// <summary>
        /// Writes a <see cref="long"/> to the stream using the given endianness.
        /// </summary>
        public void WriteInt64(long value, Endian endian)
            => WriteCore(endian.SetLong(smallBuffer, value), sizeof(long));

        /// <summary>
        /// Writes a <see cref="long"/> to the stream using little-endian.
        /// </summary>
        public void WriteInt64(long value)
            => WriteInt64(value, Endian.Little);

        /// <summary>
        /// Reads a <see cref="ulong"/> from the stream using the given endianness.
        /// </summary>
        public ulong ReadUInt64(Endian endian)
            => endian.GetULong(ReadCore(sizeof(ulong)));

        /// <summary>
        /// Reads a <see cref="ulong"/> from the stream using little-endian.
        /// </summary>
        public ulong ReadUInt64()
            => ReadUInt64(Endian.Little);

        /// <summary>
        /// Writes a <see cref="ulong"/> to the stream using the given endianness.
        /// </summary>
        public void WriteUInt64(ulong value, Endian endian)
            => WriteCore(endian.SetULong(smallBuffer, value), sizeof(ulong));

        /// <summary>
        /// Writes a <see cref="ulong"/> to the stream using little-endian.
        /// </summary>
        public void WriteUInt64(ulong value)
            => WriteUInt64(value, Endian.Little);

        /// <summary>
        /// Reads a <see cref="float"/> from the stream using the given endianness.
        /// </summary>
        public float ReadSingle(Endian endian)
            => endian.GetFloat(ReadCore(sizeof(float)));

        /// <summary>
        /// Reads a <see cref="float"/> from the stream using little-endian.
        /// </summary>
        public float ReadSingle()
            => ReadSingle(Endian.Little);

        /// <summary>
        /// Writes a <see cref="float"/> to the stream using the given endianness.
        /// </summary>
        public void WriteSingle(float value, Endian endian)
            => WriteCore(endian.SetFloat(smallBuffer, value), sizeof(float));

        /// <summary>
        /// Writes a <see cref="float"/> to the stream using little-endian.
        /// </summary>
        public void WriteSingle(float value)
            => WriteSingle(value, Endian.Little);

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
            => WriteCore(endian.SetDouble(smallBuffer, value), sizeof(double));

        /// <summary>
        /// Writes a <see cref="double"/> to the stream using little-endian.
        /// </summary>
        public void WriteDouble(double value)
            => WriteDouble(value, Endian.Little);

        /// <summary>
        /// Reads a <see cref="DateTime"/> from the stream using the given endianness.
        /// </summary>
        public DateTime ReadDateTime(Endian endian)
            => DateTime.FromBinary(ReadInt64(endian));

        /// <summary>
        /// Reads a <see cref="DateTime"/> from the stream using little-endian.
        /// </summary>
        public DateTime ReadDateTime()
            => ReadDateTime(Endian.Little);

        /// <summary>
        /// Writes a <see cref="DateTime"/> to the stream using the given endianness.
        /// </summary>
        public void WriteDateTime(DateTime value, Endian endian)
            => WriteInt64(value.ToBinary(), endian);

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
            int length = ReadInt32(endian);
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

            WriteInt32(length, endian);
            WriteCore(bytes, length);
        }

        /// <summary>
        /// Writes a <see cref="string"/> to the stream using little-endian.
        /// </summary>
        public void WriteString(string value)
            => WriteString(value, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="bool"/> using the given <paramref name="endian"/>.
        /// </summary>
        public bool[] ReadBoolArray(Endian endian)
            => ReadArrayCore(_ => ReadBool(), endian);

        /// <summary>
        /// Reads an array of <see cref="bool"/> using little-endian.
        /// </summary>
        public bool[] ReadBoolArray()
            => ReadBoolArray(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="bool"/> using the given <paramref name="endian"/>.
        /// </summary>
        public void WriteBoolArray(bool[] values, Endian endian)
            => WriteArrayCore(values, (x, _) => { WriteBool(x); }, endian);

        /// <summary>
        /// Writes an array of <see cref="bool"/> using little-endian.
        /// </summary>
        public void WriteBoolArray(bool[] values)
            => WriteBoolArray(values, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="byte"/> using the given <paramref name="endian"/>.
        /// </summary>
        public byte[] ReadUInt8Array(Endian endian)
            => ReadArrayCore(_ => ReadUInt8(), endian);

        /// <summary>
        /// Reads an array of <see cref="byte"/> using little-endian.
        /// </summary>
        public byte[] ReadUInt8Array()
            => ReadUInt8Array(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="byte"/> using the given <paramref name="endian"/>.
        /// </summary>
        public void WriteUInt8Array(byte[] values, Endian endian)
            => WriteArrayCore(values, (x, _) => { WriteUInt8(x); }, endian);

        /// <summary>
        /// Writes an array of <see cref="byte"/> using little-endian.
        /// </summary>
        public void WriteUInt8Array(byte[] values)
            => WriteUInt8Array(values, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="sbyte"/> using the given <paramref name="endian"/>.
        /// </summary>
        public sbyte[] ReadInt8Array(Endian endian)
            => ReadArrayCore(_ => ReadInt8(), endian);

        /// <summary>
        /// Reads an array of <see cref="sbyte"/> using little-endian.
        /// </summary>
        public sbyte[] ReadInt8Array()
            => ReadInt8Array(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="sbyte"/> using the given <paramref name="endian"/>.
        /// </summary>
        public void WriteInt8Array(sbyte[] values, Endian endian)
            => WriteArrayCore(values, (x, _) => { WriteInt8(x); }, endian);

        /// <summary>
        /// Writes an array of <see cref="byte"/> using little-endian.
        /// </summary>
        public void WriteInt8Array(sbyte[] values)
            => WriteInt8Array(values, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="short"/> using the given <paramref name="endian"/>.
        /// </summary>
        public short[] ReadInt16Array(Endian endian)
            => ReadArrayCore(ReadInt16, endian);

        /// <summary>
        /// Reads an array of <see cref="short"/> using little-endian.
        /// </summary>
        public short[] ReadInt16Array()
            => ReadInt16Array(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="short"/> using the given <paramref name="endian"/>.
        /// </summary>
        public void WriteInt16Array(short[] values, Endian endian)
            => WriteArrayCore(values, WriteInt16, endian);

        /// <summary>
        /// Writes an array of <see cref="short"/> using little-endian.
        /// </summary>
        public void WriteInt16Array(short[] values)
            => WriteInt16Array(values, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="ushort"/> using the given <paramref name="endian"/>.
        /// </summary>
        public ushort[] ReadUInt16Array(Endian endian)
            => ReadArrayCore(ReadUInt16, endian);

        /// <summary>
        /// Reads an array of <see cref="ushort"/> using little-endian.
        /// </summary>
        public ushort[] ReadUInt16Array()
            => ReadUInt16Array(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="ushort"/> using the given <paramref name="endian"/>.
        /// </summary>
        public void WriteUInt16Array(ushort[] values, Endian endian)
            => WriteArrayCore(values, WriteUInt16, endian);

        /// <summary>
        /// Writes an array of <see cref="ushort"/> using little-endian.
        /// </summary>
        public void WriteUInt16Array(ushort[] values)
            => WriteUInt16Array(values, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="int"/> using the given <paramref name="endian"/>.
        /// </summary>
        public int[] ReadInt32Array(Endian endian)
            => ReadArrayCore(ReadInt32, endian);

        /// <summary>
        /// Reads an array of <see cref="int"/> using little-endian.
        /// </summary>
        public int[] ReadInt32Array()
            => ReadInt32Array(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="int"/> using the given <paramref name="endian"/>.
        /// </summary>
        public void WriteInt32Array(int[] values, Endian endian)
            => WriteArrayCore(values, WriteInt32, endian);

        /// <summary>
        /// Writes an array of <see cref="int"/> using little-endian.
        /// </summary>
        public void WriteInt32Array(int[] values)
            => WriteInt32Array(values, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="uint"/> using the given <paramref name="endian"/>.
        /// </summary>
        public uint[] ReadUInt32Array(Endian endian)
            => ReadArrayCore(ReadUInt32, endian);

        /// <summary>
        /// Reads an array of <see cref="uint"/> using little-endian.
        /// </summary>
        public uint[] ReadUInt32Array()
            => ReadUInt32Array(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="uint"/> using the given <paramref name="endian"/>.
        /// </summary>
        public void WriteUInt32Array(uint[] values, Endian endian)
            => WriteArrayCore(values, WriteUInt32, endian);

        /// <summary>
        /// Writes an array of <see cref="uint"/> using little-endian.
        /// </summary>
        public void WriteUInt32Array(uint[] values)
            => WriteUInt32Array(values, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="long"/> using the given <paramref name="endian"/>.
        /// </summary>
        public long[] ReadInt64Array(Endian endian)
            => ReadArrayCore(ReadInt64, endian);

        /// <summary>
        /// Reads an array of <see cref="long"/> using little-endian.
        /// </summary>
        public long[] ReadInt64Array()
            => ReadInt64Array(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="long"/> using the given <paramref name="endian"/>.
        /// </summary>
        public void WriteInt64Array(long[] values, Endian endian)
            => WriteArrayCore(values, WriteInt64, endian);

        /// <summary>
        /// Writes an array of <see cref="long"/> using little-endian.
        /// </summary>
        public void WriteInt64Array(long[] values)
            => WriteInt64Array(values, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="ulong"/> using the given <paramref name="endian"/>.
        /// </summary>
        public ulong[] ReadUInt64Array(Endian endian)
            => ReadArrayCore(ReadUInt64, endian);

        /// <summary>
        /// Reads an array of <see cref="ulong"/> using little-endian.
        /// </summary>
        public ulong[] ReadUInt64Array()
            => ReadUInt64Array(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="ulong"/> using the given <paramref name="endian"/>.
        /// </summary>
        public void WriteUInt64Array(ulong[] values, Endian endian)
            => WriteArrayCore(values, WriteUInt64, endian);

        /// <summary>
        /// Writes an array of <see cref="ulong"/> using little-endian.
        /// </summary>
        public void WriteUInt64Array(ulong[] values)
            => WriteUInt64Array(values, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="float"/> using the given <paramref name="endian"/>.
        /// </summary>
        public float[] ReadSingleArray(Endian endian)
            => ReadArrayCore(ReadSingle, endian);

        /// <summary>
        /// Reads an array of <see cref="float"/> using little-endian.
        /// </summary>
        public float[] ReadSingleArray()
            => ReadSingleArray(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="float"/> using the given <paramref name="endian"/>.
        /// </summary>
        public void WriteSingleArray(float[] values, Endian endian)
            => WriteArrayCore(values, WriteSingle, endian);

        /// <summary>
        /// Writes an array of <see cref="float"/> using little-endian.
        /// </summary>
        public void WriteSingleArray(float[] values)
            => WriteSingleArray(values, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="double"/> using the given <paramref name="endian"/>.
        /// </summary>
        public double[] ReadDoubleArray(Endian endian)
            => ReadArrayCore(ReadDouble, endian);

        /// <summary>
        /// Reads an array of <see cref="double"/> using little-endian.
        /// </summary>
        public double[] ReadDoubleArray()
            => ReadDoubleArray(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="double"/> using the given <paramref name="endian"/>.
        /// </summary>
        public void WriteDoubleArray(double[] values, Endian endian)
            => WriteArrayCore(values, WriteDouble, endian);

        /// <summary>
        /// Writes an array of <see cref="double"/> using little-endian.
        /// </summary>
        public void WriteDoubleArray(double[] values)
            => WriteDoubleArray(values, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="DateTime"/> using the given <paramref name="endian"/>.
        /// </summary>
        public DateTime[] ReadDateTimeArray(Endian endian)
            => ReadArrayCore(ReadDateTime, endian);

        /// <summary>
        /// Reads an array of <see cref="DateTime"/> using little-endian.
        /// </summary>
        public DateTime[] ReadDateTimeArray()
            => ReadDateTimeArray(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="DateTime"/> using the given <paramref name="endian"/>.
        /// </summary>
        public void WriteDateTimeArray(DateTime[] values, Endian endian)
            => WriteArrayCore(values, WriteDateTime, endian);

        /// <summary>
        /// Writes an array of <see cref="DateTime"/> using little-endian.
        /// </summary>
        public void WriteDateTimeArray(DateTime[] values)
            => WriteDateTimeArray(values, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="string"/> using the given <paramref name="endian"/>.
        /// </summary>
        public string[] ReadStringArray(Endian endian)
            => ReadArrayCore(ReadString, endian);

        /// <summary>
        /// Reads an array of <see cref="string"/> using little-endian.
        /// </summary>
        public string[] ReadStringArray()
            => ReadStringArray(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="string"/> using the given <paramref name="endian"/>.
        /// </summary>
        public void WriteStringArray(string[] values, Endian endian)
            => WriteArrayCore(values, WriteString, endian);

        /// <summary>
        /// Writes an array of <see cref="string"/> using little-endian.
        /// </summary>
        public void WriteStringArray(string[] values)
            => WriteStringArray(values, Endian.Little);

        /// <summary>
        /// Reads an array of type <typeparamref name="T"/>.
        /// </summary>
        public T[] ReadArray<T>(Func<Endian, T> read, Endian endian)
            => ReadArrayCore(read, endian);

        /// <summary>
        /// Reads an array of type <typeparamref name="T"/>.
        /// <para>
        /// Note: The <paramref name="read"/> function must use little-endian.
        /// </para>
        /// </summary>
        /// <param name="read">Function reading one element.</param>
        public T[] ReadArray<T>(Func<T> read)
            => ReadArrayCore(_ => read(), Endian.Little);

        /// <summary>
        /// Writes an array of type <typeparamref name="T"/> using the given <paramref name="endian"/>.
        /// </summary>
        public void WriteArray<T>(T[] array, Action<T, Endian> write, Endian endian)
            => WriteArrayCore(array, write, endian);

        /// <summary>
        /// Writes an array of type <typeparamref name="T"/> using  using little-endian.
        /// <para>
        /// Note: The <paramref name="write"/> action must use little-endian.
        /// </para>
        /// </summary>
        public void WriteArray<T>(T[] array, Action<T> write)
            => WriteArrayCore(array, (x, _) => { write(x); }, Endian.Little);

        /// <summary>
        /// Reads a dictionary using the given <paramref name="endian"/>.
        /// </summary>
        public Dictionary<K, V> ReadDictionary<K, V>(Func<Endian, K> readKey, Func<Endian, V> readValue, Endian endian)
            where K : notnull, IComparable<K>
        {
            int length = ReadInt32(endian);
            Dictionary<K, V> result = [];

            for (int i = 0; i < length; ++i)
            {
                K key = readKey(endian);
                V value = readValue(endian);

                result.Add(key, value);
            }

            return result;
        }

        /// <summary>
        /// Reads a dictionary using little-endian.
        /// <para>
        /// Note: The <paramref name="readKey"/> and <paramref name="readValue"/> functions must use little-endian.
        /// </para>
        /// </summary>
        public Dictionary<K, V> ReadDictionary<K, V>(Func<K> readKey, Func<V> readValue)
            where K : notnull, IComparable<K>
            => ReadDictionary(_1 => readKey(), _2 => readValue(), Endian.Little);

        /// <summary>
        /// Writes a dictionary using the given <paramref name="endian"/>.
        /// </summary>
        public void WriteDictionary<K, V>(Dictionary<K, V> dictionary, Action<K, Endian> writeKey, Action<V, Endian> writeValue, Endian endian)
            where K : notnull, IComparable<K>
        {
            KeyValuePair<K, V>[] kvps = [.. dictionary.OrderBy(x => x.Key)];

            WriteInt32(kvps.Length, endian);
            kvps.Apply(x => { writeKey(x.Key, endian); writeValue(x.Value, endian); });
        }

        /// <summary>
        /// Writes a dictionary using little-endian.
        /// <para>
        /// Note: The <paramref name="writeKey"/> and <paramref name="writeValue"/> actions must use little-endian.
        /// </para>
        /// </summary>
        public void WriteDictionary<K, V>(Dictionary<K, V> dictionary, Action<K> writeKey, Action<V> writeValue)
            where K : notnull, IComparable<K>
            => WriteDictionary(dictionary, (k, e) => { writeKey(k); }, (v, e) => { writeValue(v); }, Endian.Little);

        private byte[] ReadCore(int byteCount)
            => ReadCore(smallBuffer, byteCount);

        private byte[] ReadCore(byte[] buffer, int byteCount)
        {
            if (stream.Read(buffer, 0, byteCount) != byteCount)
            {
                throw Ex.EndOfStream();
            }

            return buffer;
        }

        private void WriteCore(ReadOnlySpan<byte> buffer, int byteCount)
        {
            stream.Write(buffer.ToArray(), 0, byteCount);
        }

        private T[] ReadArrayCore<T>(Func<Endian, T> read, Endian endian)
        {
            int length = ReadInt32(endian);

            return Enumerable.Range(0, length).Select(_ => read(endian)).ToArray();
        }

        private void WriteArrayCore<T>(T[] array, Action<T, Endian> write, Endian endian)
        {
            int length = array.Length;

            WriteInt32(length, endian);

            Enumerable.Range(0, length).Apply(x => { write(array[x], endian); });
        }
    }
}
