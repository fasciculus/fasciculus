using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fasciculus.IO.Binary
{
    /// <summary>
    /// Converters to and from byte arrays for little-endian and big-endian.
    /// </summary>
    public abstract class Endian
    {
        private static readonly Endian straight = new Straight();
        private static readonly Endian reverted = new Reverted();

        /// <summary>
        /// Little-endian converter.
        /// </summary>
        public static Endian Little { get; } = BitConverter.IsLittleEndian ? straight : reverted;

        /// <summary>
        /// Big-endian converter.
        /// </summary>
        public static Endian Big { get; } = BitConverter.IsLittleEndian ? reverted : straight;

        /// <summary>
        /// Converter using current endianness.
        /// </summary>
        public static Endian Current { get; } = straight;

        /// <summary>
        /// Reads the first 2 bytes of the given <paramref name="buffer"/> and converts them to a <see cref="short"/>
        /// </summary>
        /// <returns></returns>
        public abstract short GetInt16(ReadOnlySpan<byte> buffer);

        /// <summary>
        /// Converts the given <paramref name="value"/> into 2 bytes and writes them to the beginning of the given <paramref name="buffer"/>.
        /// </summary>
        public abstract Span<byte> SetInt16(Span<byte> buffer, short value);

        /// <summary>
        /// Reads the first 2 bytes of the given <paramref name="buffer"/> and converts them to a <see cref="ushort"/>
        /// </summary>
        /// <returns></returns>
        public abstract ushort GetUInt16(ReadOnlySpan<byte> buffer);

        /// <summary>
        /// Converts the given <paramref name="value"/> into 2 bytes and writes them to the beginning of the given <paramref name="buffer"/>.
        /// </summary>
        public abstract Span<byte> SetUInt16(Span<byte> buffer, ushort value);

        /// <summary>
        /// Reads the first 4 bytes of the given <paramref name="buffer"/> and converts them to a <see cref="int"/>
        /// </summary>
        public abstract int GetInt32(ReadOnlySpan<byte> buffer);

        /// <summary>
        /// Converts the given <paramref name="value"/> into 4 bytes and writes them to the beginning of the given <paramref name="buffer"/>.
        /// </summary>
        public abstract Span<byte> SetInt32(Span<byte> buffer, int value);

        /// <summary>
        /// Reads the first 4 bytes of the given <paramref name="buffer"/> and converts them to a <see cref="uint"/>
        /// </summary>
        public abstract uint GetUInt32(ReadOnlySpan<byte> buffer);

        /// <summary>
        /// Converts the given <paramref name="value"/> into 4 bytes and writes them to the beginning of the given <paramref name="buffer"/>.
        /// </summary>
        public abstract Span<byte> SetUInt32(Span<byte> buffer, uint value);

        /// <summary>
        /// Reads the first 8 bytes of the given <paramref name="buffer"/> and converts them to a <see cref="long"/>
        /// </summary>
        public abstract long GetInt64(ReadOnlySpan<byte> buffer);

        /// <summary>
        /// Converts the given <paramref name="value"/> into 8 bytes and writes them to the beginning of the given <paramref name="buffer"/>.
        /// </summary>
        public abstract Span<byte> SetInt64(Span<byte> buffer, long value);

        /// <summary>
        /// Reads the first 8 bytes of the given <paramref name="buffer"/> and converts them to a <see cref="ulong"/>
        /// </summary>
        public abstract ulong GetUInt64(ReadOnlySpan<byte> buffer);

        /// <summary>
        /// Converts the given <paramref name="value"/> into 8 bytes and writes them to the beginning of the given <paramref name="buffer"/>.
        /// </summary>
        public abstract Span<byte> SetUInt64(Span<byte> buffer, ulong value);

        /// <summary>
        /// Reads the first 4 bytes of the given <paramref name="buffer"/> and converts them to a <see cref="float"/>
        /// </summary>
        public abstract float GetSingle(ReadOnlySpan<byte> buffer);

        /// <summary>
        /// Converts the given <paramref name="value"/> into 4 bytes and writes them to the beginning of the given <paramref name="buffer"/>.
        /// </summary>
        public abstract Span<byte> SetSingle(Span<byte> buffer, float value);

        /// <summary>
        /// Reads the first 8 bytes of the given <paramref name="buffer"/> and converts them to a <see cref="double"/>
        /// </summary>
        public abstract double GetDouble(ReadOnlySpan<byte> buffer);

        /// <summary>
        /// Converts the given <paramref name="value"/> into 8 bytes and writes them to the beginning of the given <paramref name="buffer"/>.
        /// </summary>
        public abstract Span<byte> SetDouble(Span<byte> buffer, double value);

        private class Straight : Endian
        {
            public override short GetInt16(ReadOnlySpan<byte> buffer)
                => GetCore<short>(buffer, sizeof(short));

            public override Span<byte> SetInt16(Span<byte> buffer, short value)
                => SetCore(buffer, value, sizeof(short));

            public override ushort GetUInt16(ReadOnlySpan<byte> buffer)
                => GetCore<ushort>(buffer, sizeof(ushort));

            public override Span<byte> SetUInt16(Span<byte> buffer, ushort value)
                => SetCore(buffer, value, sizeof(ushort));

            public override int GetInt32(ReadOnlySpan<byte> buffer)
                => GetCore<int>(buffer, sizeof(int));

            public override Span<byte> SetInt32(Span<byte> buffer, int value)
                => SetCore(buffer, value, sizeof(int));

            public override uint GetUInt32(ReadOnlySpan<byte> buffer)
                => GetCore<uint>(buffer, sizeof(uint));

            public override Span<byte> SetUInt32(Span<byte> buffer, uint value)
                => SetCore(buffer, value, sizeof(uint));

            public override long GetInt64(ReadOnlySpan<byte> buffer)
                => GetCore<long>(buffer, sizeof(long));

            public override Span<byte> SetInt64(Span<byte> buffer, long value)
                => SetCore(buffer, value, sizeof(long));

            public override ulong GetUInt64(ReadOnlySpan<byte> buffer)
                => GetCore<ulong>(buffer, sizeof(ulong));

            public override Span<byte> SetUInt64(Span<byte> buffer, ulong value)
                => SetCore(buffer, value, sizeof(ulong));

            public override float GetSingle(ReadOnlySpan<byte> buffer)
                => GetCore<float>(buffer, sizeof(float));

            public override Span<byte> SetSingle(Span<byte> buffer, float value)
                => SetCore(buffer, value, sizeof(float));

            public override double GetDouble(ReadOnlySpan<byte> buffer)
                => GetCore<double>(buffer, sizeof(double));

            public override Span<byte> SetDouble(Span<byte> buffer, double value)
                => SetCore(buffer, value, sizeof(double));

            private static T GetCore<T>(ReadOnlySpan<byte> buffer, int size)
            {
                return Unsafe.ReadUnaligned<T>(ref MemoryMarshal.GetReference(CheckBuffer(buffer, size)));
            }

            private static Span<byte> SetCore<T>(Span<byte> buffer, T value, int size)
            {
                Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(CheckBuffer(buffer, size)), value);

                return buffer;
            }

            private static ReadOnlySpan<byte> CheckBuffer(ReadOnlySpan<byte> buffer, int requiredLength)
            {
                if (buffer.Length < requiredLength)
                {
                    throw new IndexOutOfRangeException();
                }

                return buffer;
            }

            private static Span<byte> CheckBuffer(Span<byte> buffer, int requiredLength)
            {
                if (buffer.Length < requiredLength)
                {
                    throw new IndexOutOfRangeException();
                }

                return buffer;
            }
        }

        private class Reverted : Endian
        {
            public override short GetInt16(ReadOnlySpan<byte> buffer)
                => straight.GetInt16(ReverseInput(buffer, sizeof(short)));

            public override Span<byte> SetInt16(Span<byte> buffer, short value)
                => ReverseOutput(straight.SetInt16(buffer, value), sizeof(short));

            public override ushort GetUInt16(ReadOnlySpan<byte> buffer)
                => straight.GetUInt16(ReverseInput(buffer, sizeof(ushort)));

            public override Span<byte> SetUInt16(Span<byte> buffer, ushort value)
                => ReverseOutput(straight.SetUInt16(buffer, value), sizeof(short));

            public override int GetInt32(ReadOnlySpan<byte> buffer)
                => straight.GetInt32(ReverseInput(buffer, sizeof(int)));

            public override Span<byte> SetInt32(Span<byte> buffer, int value)
                => ReverseOutput(straight.SetInt32(buffer, value), sizeof(int));

            public override uint GetUInt32(ReadOnlySpan<byte> buffer)
                => straight.GetUInt32(ReverseInput(buffer, sizeof(uint)));

            public override Span<byte> SetUInt32(Span<byte> buffer, uint value)
                => ReverseOutput(straight.SetUInt32(buffer, value), sizeof(uint));

            public override long GetInt64(ReadOnlySpan<byte> buffer)
                => straight.GetInt64(ReverseInput(buffer, sizeof(long)));

            public override Span<byte> SetInt64(Span<byte> buffer, long value)
                => ReverseOutput(straight.SetInt64(buffer, value), sizeof(long));

            public override ulong GetUInt64(ReadOnlySpan<byte> buffer)
                => straight.GetUInt64(ReverseInput(buffer, sizeof(ulong)));

            public override Span<byte> SetUInt64(Span<byte> buffer, ulong value)
                => ReverseOutput(straight.SetUInt64(buffer, value), sizeof(ulong));

            public override float GetSingle(ReadOnlySpan<byte> buffer)
                => straight.GetSingle(ReverseInput(buffer, sizeof(float)));

            public override Span<byte> SetSingle(Span<byte> buffer, float value)
                => ReverseOutput(straight.SetSingle(buffer, value), sizeof(float));

            public override double GetDouble(ReadOnlySpan<byte> buffer)
                => straight.GetDouble(ReverseInput(buffer, sizeof(double)));

            public override Span<byte> SetDouble(Span<byte> buffer, double value)
                => ReverseOutput(straight.SetDouble(buffer, value), sizeof(double));

            private static byte[] ReverseInput(ReadOnlySpan<byte> source, int count)
            {
                byte[] reversed = new byte[count];

                for (int i = 0, j = count - 1; i < count; ++i, --j)
                {
                    reversed[i] = source[j];
                }

                return reversed;
            }

            private static Span<byte> ReverseOutput(Span<byte> buffer, int count)
            {
                buffer.Reverse();

                return buffer;
            }
        }
    }
}
