using Fasciculus.Support;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fasciculus.IO
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
        public static readonly Endian Little = BitConverter.IsLittleEndian ? straight : reverted;

        /// <summary>
        /// Big-endian converter.
        /// </summary>
        public static readonly Endian Big = BitConverter.IsLittleEndian ? reverted : straight;

        /// <summary>
        /// Converter using current endianness.
        /// </summary>
        public static readonly Endian Current = straight;

        /// <summary>
        /// Reads the first 2 bytes of the given <paramref name="buffer"/> and converts them to a <see cref="short"/>
        /// </summary>
        /// <returns></returns>
        public abstract short GetShort(ReadOnlySpan<byte> buffer);

        /// <summary>
        /// Converts the given <paramref name="value"/> into 2 bytes and writes them to the beginning of the given <paramref name="buffer"/>.
        /// </summary>
        public abstract Span<byte> SetShort(Span<byte> buffer, short value);

        /// <summary>
        /// Reads the first 2 bytes of the given <paramref name="buffer"/> and converts them to a <see cref="ushort"/>
        /// </summary>
        /// <returns></returns>
        public abstract ushort GetUShort(ReadOnlySpan<byte> buffer);

        /// <summary>
        /// Converts the given <paramref name="value"/> into 2 bytes and writes them to the beginning of the given <paramref name="buffer"/>.
        /// </summary>
        public abstract Span<byte> SetUShort(Span<byte> buffer, ushort value);

        /// <summary>
        /// Reads the first 4 bytes of the given <paramref name="buffer"/> and converts them to a <see cref="int"/>
        /// </summary>
        public abstract int GetInt(ReadOnlySpan<byte> buffer);

        /// <summary>
        /// Converts the given <paramref name="value"/> into 4 bytes and writes them to the beginning of the given <paramref name="buffer"/>.
        /// </summary>
        public abstract Span<byte> SetInt(Span<byte> buffer, int value);

        /// <summary>
        /// Reads the first 4 bytes of the given <paramref name="buffer"/> and converts them to a <see cref="uint"/>
        /// </summary>
        public abstract uint GetUInt(ReadOnlySpan<byte> buffer);

        /// <summary>
        /// Converts the given <paramref name="value"/> into 4 bytes and writes them to the beginning of the given <paramref name="buffer"/>.
        /// </summary>
        public abstract Span<byte> SetUInt(Span<byte> buffer, uint value);

        /// <summary>
        /// Reads the first 8 bytes of the given <paramref name="buffer"/> and converts them to a <see cref="long"/>
        /// </summary>
        public abstract long GetLong(ReadOnlySpan<byte> buffer);

        /// <summary>
        /// Converts the given <paramref name="value"/> into 8 bytes and writes them to the beginning of the given <paramref name="buffer"/>.
        /// </summary>
        public abstract Span<byte> SetLong(Span<byte> buffer, long value);

        /// <summary>
        /// Reads the first 8 bytes of the given <paramref name="buffer"/> and converts them to a <see cref="ulong"/>
        /// </summary>
        public abstract ulong GetULong(ReadOnlySpan<byte> buffer);

        /// <summary>
        /// Converts the given <paramref name="value"/> into 8 bytes and writes them to the beginning of the given <paramref name="buffer"/>.
        /// </summary>
        public abstract Span<byte> SetULong(Span<byte> buffer, ulong value);

        /// <summary>
        /// Reads the first 4 bytes of the given <paramref name="buffer"/> and converts them to a <see cref="float"/>
        /// </summary>
        public abstract float GetFloat(ReadOnlySpan<byte> buffer);

        /// <summary>
        /// Converts the given <paramref name="value"/> into 4 bytes and writes them to the beginning of the given <paramref name="buffer"/>.
        /// </summary>
        public abstract Span<byte> SetFloat(Span<byte> buffer, float value);

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
            public override short GetShort(ReadOnlySpan<byte> buffer)
                => GetCore<short>(buffer, sizeof(short));

            public override Span<byte> SetShort(Span<byte> buffer, short value)
                => SetCore(buffer, value, sizeof(short));

            public override ushort GetUShort(ReadOnlySpan<byte> buffer)
                => GetCore<ushort>(buffer, sizeof(ushort));

            public override Span<byte> SetUShort(Span<byte> buffer, ushort value)
                => SetCore(buffer, value, sizeof(ushort));

            public override int GetInt(ReadOnlySpan<byte> buffer)
                => GetCore<int>(buffer, sizeof(int));

            public override Span<byte> SetInt(Span<byte> buffer, int value)
                => SetCore(buffer, value, sizeof(int));

            public override uint GetUInt(ReadOnlySpan<byte> buffer)
                => GetCore<uint>(buffer, sizeof(uint));

            public override Span<byte> SetUInt(Span<byte> buffer, uint value)
                => SetCore(buffer, value, sizeof(uint));

            public override long GetLong(ReadOnlySpan<byte> buffer)
                => GetCore<long>(buffer, sizeof(long));

            public override Span<byte> SetLong(Span<byte> buffer, long value)
                => SetCore(buffer, value, sizeof(long));

            public override ulong GetULong(ReadOnlySpan<byte> buffer)
                => GetCore<ulong>(buffer, sizeof(ulong));

            public override Span<byte> SetULong(Span<byte> buffer, ulong value)
                => SetCore(buffer, value, sizeof(ulong));

            public override float GetFloat(ReadOnlySpan<byte> buffer)
                => GetCore<float>(buffer, sizeof(float));

            public override Span<byte> SetFloat(Span<byte> buffer, float value)
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
                    throw Ex.IndexOutOfRange();
                }

                return buffer;
            }

            private static Span<byte> CheckBuffer(Span<byte> buffer, int requiredLength)
            {
                if (buffer.Length < requiredLength)
                {
                    throw Ex.IndexOutOfRange();
                }

                return buffer;
            }
        }

        private class Reverted : Endian
        {
            public override short GetShort(ReadOnlySpan<byte> buffer)
                => straight.GetShort(ReverseInput(buffer, sizeof(short)));

            public override Span<byte> SetShort(Span<byte> buffer, short value)
                => ReverseOutput(straight.SetShort(buffer, value), sizeof(short));

            public override ushort GetUShort(ReadOnlySpan<byte> buffer)
                => straight.GetUShort(ReverseInput(buffer, sizeof(ushort)));

            public override Span<byte> SetUShort(Span<byte> buffer, ushort value)
                => ReverseOutput(straight.SetUShort(buffer, value), sizeof(short));

            public override int GetInt(ReadOnlySpan<byte> buffer)
                => straight.GetInt(ReverseInput(buffer, sizeof(int)));

            public override Span<byte> SetInt(Span<byte> buffer, int value)
                => ReverseOutput(straight.SetInt(buffer, value), sizeof(int));

            public override uint GetUInt(ReadOnlySpan<byte> buffer)
                => straight.GetUInt(ReverseInput(buffer, sizeof(uint)));

            public override Span<byte> SetUInt(Span<byte> buffer, uint value)
                => ReverseOutput(straight.SetUInt(buffer, value), sizeof(uint));

            public override long GetLong(ReadOnlySpan<byte> buffer)
                => straight.GetLong(ReverseInput(buffer, sizeof(long)));

            public override Span<byte> SetLong(Span<byte> buffer, long value)
                => ReverseOutput(straight.SetLong(buffer, value), sizeof(long));

            public override ulong GetULong(ReadOnlySpan<byte> buffer)
                => straight.GetULong(ReverseInput(buffer, sizeof(ulong)));

            public override Span<byte> SetULong(Span<byte> buffer, ulong value)
                => ReverseOutput(straight.SetULong(buffer, value), sizeof(ulong));

            public override float GetFloat(ReadOnlySpan<byte> buffer)
                => straight.GetFloat(ReverseInput(buffer, sizeof(float)));

            public override Span<byte> SetFloat(Span<byte> buffer, float value)
                => ReverseOutput(straight.SetFloat(buffer, value), sizeof(float));

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
                for (int i = 0, j = count - 1; i < j; ++i, --j)
                {
                    (buffer[i], buffer[j]) = (buffer[j], buffer[i]);
                }

                return buffer;
            }
        }
    }
}
