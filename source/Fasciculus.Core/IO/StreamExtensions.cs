using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.IO
{
    public static class StreamExtensions
    {
        public static bool ReadBool(this Stream stream)
        {
            byte[] buffer = new byte[1];

            stream.Read(buffer, 0, buffer.Length);

            return BitConverter.ToBoolean(buffer, 0);
        }

        public static void WriteBool(this Stream stream, bool value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            stream.Write(bytes, 0, bytes.Length);
        }

        public static short ReadShort(this Stream stream)
        {
            byte[] buffer = new byte[2];

            stream.Read(buffer, 0, buffer.Length);

            return BitConverter.ToInt16(buffer, 0);
        }

        public static void WriteShort(this Stream stream, short value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            stream.Write(bytes, 0, bytes.Length);
        }

        public static int ReadInt(this Stream stream)
        {
            byte[] buffer = new byte[4];

            stream.Read(buffer, 0, buffer.Length);

            return BitConverter.ToInt32(buffer, 0);
        }

        public static void WriteInt(this Stream stream, int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            stream.Write(bytes, 0, bytes.Length);
        }

        public static double ReadDouble(this Stream stream)
        {
            byte[] buffer = new byte[8];

            stream.Read(buffer, 0, buffer.Length);

            return BitConverter.ToDouble(buffer, 0);
        }

        public static void WriteDouble(this Stream stream, double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            stream.Write(bytes, 0, bytes.Length);
        }

        public static string ReadString(this Stream stream)
        {
            int length = stream.ReadInt();
            byte[] buffer = new byte[length];
            stream.Read(buffer, 0, buffer.Length);

            return Encoding.UTF8.GetString(buffer, 0, length);
        }

        public static void WriteString(this Stream stream, string value)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(value);

            stream.WriteInt(buffer.Length);
            stream.Write(buffer, 0, buffer.Length);
        }

        public static short[] ReadShortArray(this Stream stream)
        {
            int length = stream.ReadInt();

            return Enumerable.Range(0, length).Select(_ => stream.ReadShort()).ToArray();
        }

        public static void WriteShortArray(this Stream stream, short[] values)
        {
            stream.WriteInt(values.Length);
            values.Apply(stream.WriteShort);
        }

        public static T[] ReadArray<T>(this Stream stream, Func<Stream, T> read)
        {
            int length = stream.ReadInt();

            return Enumerable.Range(0, length).Select(i => read(stream)).ToArray();
        }

        public static void WriteArray<T>(this Stream stream, T[] values, Action<T> write)
        {
            stream.WriteInt(values.Length);

            values.Apply(value => write(value));
        }
    }
}
