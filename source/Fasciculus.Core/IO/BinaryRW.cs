using Fasciculus.Collections;
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
            => ReadByte() != 0;

        /// <summary>
        /// Writes a <see cref="bool"/> to the stream.
        /// </summary>
        public void WriteBool(bool value)
            => WriteByte((byte)(value ? 1 : 0));

        /// <summary>
        /// Reads a <see cref="byte"/> from the stream
        /// </summary>
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
            => WriteCore(endian.SetShort(smallBuffer, value), sizeof(short));

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
            => WriteCore(endian.SetUShort(smallBuffer, value), sizeof(short));

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
            => WriteCore(endian.SetInt(smallBuffer, value), sizeof(int));

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
            => WriteCore(endian.SetUInt(smallBuffer, value), sizeof(uint));

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
            => WriteCore(endian.SetLong(smallBuffer, value), sizeof(long));

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
            => WriteCore(endian.SetULong(smallBuffer, value), sizeof(ulong));

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
            => WriteCore(endian.SetFloat(smallBuffer, value), sizeof(float));

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
        public byte[] ReadByteArray(Endian endian)
            => ReadArrayCore(_ => ReadByte(), endian);

        /// <summary>
        /// Reads an array of <see cref="byte"/> using little-endian.
        /// </summary>
        public byte[] ReadByteArray()
            => ReadByteArray(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="byte"/> using the given <paramref name="endian"/>.
        /// </summary>
        public void WriteByteArray(byte[] values, Endian endian)
            => WriteArrayCore(values, (x, _) => { WriteByte(x); }, endian);

        /// <summary>
        /// Writes an array of <see cref="byte"/> using little-endian.
        /// </summary>
        public void WriteByteArray(byte[] values)
            => WriteByteArray(values, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="sbyte"/> using the given <paramref name="endian"/>.
        /// </summary>
        public sbyte[] ReadSByteArray(Endian endian)
            => ReadArrayCore(_ => ReadSByte(), endian);

        /// <summary>
        /// Reads an array of <see cref="sbyte"/> using little-endian.
        /// </summary>
        public sbyte[] ReadSByteArray()
            => ReadSByteArray(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="sbyte"/> using the given <paramref name="endian"/>.
        /// </summary>
        public void WriteSByteArray(sbyte[] values, Endian endian)
            => WriteArrayCore(values, (x, _) => { WriteSByte(x); }, endian);

        /// <summary>
        /// Writes an array of <see cref="byte"/> using little-endian.
        /// </summary>
        public void WriteSByteArray(sbyte[] values)
            => WriteSByteArray(values, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="short"/> using the given <paramref name="endian"/>.
        /// </summary>
        public short[] ReadShortArray(Endian endian)
            => ReadArrayCore(ReadShort, endian);

        /// <summary>
        /// Reads an array of <see cref="short"/> using little-endian.
        /// </summary>
        public short[] ReadShortArray()
            => ReadShortArray(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="short"/> using the given <paramref name="endian"/>.
        /// </summary>
        public void WriteShortArray(short[] values, Endian endian)
            => WriteArrayCore(values, WriteShort, endian);

        /// <summary>
        /// Writes an array of <see cref="short"/> using little-endian.
        /// </summary>
        public void WriteShortArray(short[] values)
            => WriteShortArray(values, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="ushort"/> using the given <paramref name="endian"/>.
        /// </summary>
        public ushort[] ReadUShortArray(Endian endian)
            => ReadArrayCore(ReadUShort, endian);

        /// <summary>
        /// Reads an array of <see cref="ushort"/> using little-endian.
        /// </summary>
        public ushort[] ReadUShortArray()
            => ReadUShortArray(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="ushort"/> using the given <paramref name="endian"/>.
        /// </summary>
        public void WriteUShortArray(ushort[] values, Endian endian)
            => WriteArrayCore(values, WriteUShort, endian);

        /// <summary>
        /// Writes an array of <see cref="ushort"/> using little-endian.
        /// </summary>
        public void WriteUShortArray(ushort[] values)
            => WriteUShortArray(values, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="int"/> using the given <paramref name="endian"/>.
        /// </summary>
        public int[] ReadIntArray(Endian endian)
            => ReadArrayCore(ReadInt, endian);

        /// <summary>
        /// Reads an array of <see cref="int"/> using little-endian.
        /// </summary>
        public int[] ReadIntArray()
            => ReadIntArray(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="int"/> using the given <paramref name="endian"/>.
        /// </summary>
        public void WriteIntArray(int[] values, Endian endian)
            => WriteArrayCore(values, WriteInt, endian);

        /// <summary>
        /// Writes an array of <see cref="int"/> using little-endian.
        /// </summary>
        public void WriteIntArray(int[] values)
            => WriteIntArray(values, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="uint"/> using the given <paramref name="endian"/>.
        /// </summary>
        public uint[] ReadUIntArray(Endian endian)
            => ReadArrayCore(ReadUInt, endian);

        /// <summary>
        /// Reads an array of <see cref="uint"/> using little-endian.
        /// </summary>
        public uint[] ReadUIntArray()
            => ReadUIntArray(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="uint"/> using the given <paramref name="endian"/>.
        /// </summary>
        public void WriteUIntArray(uint[] values, Endian endian)
            => WriteArrayCore(values, WriteUInt, endian);

        /// <summary>
        /// Writes an array of <see cref="uint"/> using little-endian.
        /// </summary>
        public void WriteUIntArray(uint[] values)
            => WriteUIntArray(values, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="long"/> using the given <paramref name="endian"/>.
        /// </summary>
        public long[] ReadLongArray(Endian endian)
            => ReadArrayCore(ReadLong, endian);

        /// <summary>
        /// Reads an array of <see cref="long"/> using little-endian.
        /// </summary>
        public long[] ReadLongArray()
            => ReadLongArray(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="long"/> using the given <paramref name="endian"/>.
        /// </summary>
        public void WriteLongArray(long[] values, Endian endian)
            => WriteArrayCore(values, WriteLong, endian);

        /// <summary>
        /// Writes an array of <see cref="long"/> using little-endian.
        /// </summary>
        public void WriteLongArray(long[] values)
            => WriteLongArray(values, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="ulong"/> using the given <paramref name="endian"/>.
        /// </summary>
        public ulong[] ReadULongArray(Endian endian)
            => ReadArrayCore(ReadULong, endian);

        /// <summary>
        /// Reads an array of <see cref="ulong"/> using little-endian.
        /// </summary>
        public ulong[] ReadULongArray()
            => ReadULongArray(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="ulong"/> using the given <paramref name="endian"/>.
        /// </summary>
        public void WriteULongArray(ulong[] values, Endian endian)
            => WriteArrayCore(values, WriteULong, endian);

        /// <summary>
        /// Writes an array of <see cref="ulong"/> using little-endian.
        /// </summary>
        public void WriteULongArray(ulong[] values)
            => WriteULongArray(values, Endian.Little);

        /// <summary>
        /// Reads an array of <see cref="float"/> using the given <paramref name="endian"/>.
        /// </summary>
        public float[] ReadFloatArray(Endian endian)
            => ReadArrayCore(ReadFloat, endian);

        /// <summary>
        /// Reads an array of <see cref="float"/> using little-endian.
        /// </summary>
        public float[] ReadFloatArray()
            => ReadFloatArray(Endian.Little);

        /// <summary>
        /// Writes an array of <see cref="float"/> using the given <paramref name="endian"/>.
        /// </summary>
        public void WriteFloatArray(float[] values, Endian endian)
            => WriteArrayCore(values, WriteFloat, endian);

        /// <summary>
        /// Writes an array of <see cref="float"/> using little-endian.
        /// </summary>
        public void WriteFloatArray(float[] values)
            => WriteFloatArray(values, Endian.Little);

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
            int length = ReadInt(endian);
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

            WriteInt(kvps.Length, endian);
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
            int length = ReadInt(endian);

            return Enumerable.Range(0, length).Select(_ => read(endian)).ToArray();
        }

        private void WriteArrayCore<T>(T[] array, Action<T, Endian> write, Endian endian)
        {
            int length = array.Length;

            WriteInt(length, endian);

            Enumerable.Range(0, length).Apply(x => { write(array[x], endian); });
        }
    }
}
