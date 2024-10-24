﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Fasciculus.IO
{
    public class Data
    {
        public readonly Stream stream;

        private readonly byte[] buffer = new byte[8];

        public Data(Stream stream)
        {
            this.stream = stream;
        }

        public int ReadInt()
        {
            ReadBytes(buffer, 4);

            return BitConverter.ToInt32(buffer, 0);
        }

        public void WriteInt(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            WriteBytes(bytes, 4);
        }

        public double ReadDouble()
        {
            ReadBytes(buffer, 8);

            return BitConverter.ToDouble(buffer, 0);
        }

        public void WriteDouble(double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            WriteBytes(bytes, 8);
        }

        public string ReadString()
        {
            int length = ReadInt();
            byte[] bytes = new byte[length];

            ReadBytes(bytes, length);

            return Encoding.UTF8.GetString(bytes, 0, length);
        }

        public void WriteString(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            int length = bytes.Length;

            WriteInt(length);
            WriteBytes(bytes, length);
        }

        public T[] ReadArray<T>(Func<Data, T> read)
            where T : notnull
        {
            int length = ReadInt();

            return Enumerable.Range(0, length).Select(i => read(this)).ToArray();
        }

        public void WriteArray<T>(T[] values, Action<T> write)
            where T : notnull
        {
            WriteInt(values.Length);

            values.Apply(value => write(value));
        }

        private void ReadBytes(byte[] bytes, int length)
        {
            stream.Read(bytes, 0, length);
        }

        private void WriteBytes(byte[] bytes, int length)
        {
            stream.Write(bytes, 0, length);
        }
    }
}
