using Fasciculus.Support;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fasciculus.IO
{
    /// <summary>
    /// Converters to and from byte arrays according to .
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
        /// Reads the first two bytes of the given <paramref name="buffer"/> and converts them to a <see cref="short"/>
        /// </summary>
        /// <returns></returns>
        public abstract short GetShort(ReadOnlySpan<byte> buffer);

        /// <summary>
        /// Converts the given <paramref name="value"/> into two bytes and writes them to the beginning of the given <paramref name="buffer"/>.
        /// </summary>
        public abstract Span<byte> SetShort(Span<byte> buffer, short value);

        private class Straight : Endian
        {
            public override short GetShort(ReadOnlySpan<byte> buffer)
                => Unsafe.ReadUnaligned<short>(ref MemoryMarshal.GetReference(CheckBuffer(buffer, sizeof(short))));

            public override Span<byte> SetShort(Span<byte> buffer, short value)
                => Set(buffer, value);

            private static Span<byte> Set<T>(Span<byte> buffer, T value)
            {
                Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(CheckBuffer(buffer, sizeof(short))), value);

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
