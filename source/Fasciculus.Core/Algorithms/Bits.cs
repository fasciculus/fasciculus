using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Fasciculus.Algorithms
{
    /// <summary>
    /// Provides bit related helper functions.
    /// </summary>
    public static class Bits
    {
        /// <summary>
        /// Counts the number of bits set to <c>1</c> in the given value.
        /// </summary>
        /// <returns>The count of bits set to <c>1</c> (range 0 to 8).</returns>
        public static int CountOnes(byte value)
        {
            int x = (value & 0xAA) >> 1;
            int y = value & 0x55;
            int z = x + y;

            x = (z & 0xCC) >> 2;
            y = z & 0x33;
            z = x + y;

            x = (z & 0xF0) >> 4;
            y = z & 0x0F;

            z = x + y;

            return z;
        }

        /// <summary>
        /// Returns the indices of the bits set to <c>1</c> of the given value.
        /// <para>
        /// The least significant bit has index <c>0</c>, the most significant bit has index <c>7</c>.
        /// </para>
        /// </summary>
        /// <returns>The indices of the bits set to <c>1</c>.</returns>
        public static IEnumerable<uint> Indices(byte value)
        {
            byte mask = 1;

            for (uint i = 0; i < 8; ++i, mask <<= 1)
            {
                if ((value & mask) != 0)
                {
                    yield return i;
                }
            }
        }

        /// <summary>
        /// Returns a byte in which the bits with the given indices are set to <c>1</c>.
        /// <para>
        /// The least significant bit has index <c>0</c>, the most significant bit has index <c>7</c>.
        /// </para>
        /// <para>
        /// Indices <c>&gt;7</c> are ignored.
        /// </para>
        /// </summary>
        /// <returns>The value with the according bits set to <c>1</c>.</returns>
        public static byte IndicesToByte(IEnumerable<ulong> indices)
        {
            byte result = 0;

            foreach (ulong i in indices)
            {
                if (i < 8)
                {
                    int j = (int)i;

                    result |= (byte)(1 << j);
                }
            }

            return result;
        }

        /// <summary>
        /// Counts the leading zeroes of the given value.
        /// </summary>
        /// <returns>The count of leading zeroes (range 0 to 64).</returns>
        public static int CountLeadingZeros(uint value)
        {
            if (value == 0) return 32;

            int count = 0;
            if ((value & 0xFFFF0000) == 0) { count += 16; value <<= 16; }
            if ((value & 0xFF000000) == 0) { count += 8; value <<= 8; }
            if ((value & 0xF0000000) == 0) { count += 4; value <<= 4; }
            if ((value & 0xC0000000) == 0) { count += 2; value <<= 2; }
            if ((value & 0x80000000) == 0) { count += 1; }

            return count;
        }

        /// <summary>
        /// Returns the floored binary logarithm of the given value.
        /// <para>Note: <c>Log2(0)</c> returns <c>-1</c>.</para>
        /// </summary>
        /// <returns>The logarithm (range -1 to 31)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Log2(uint value)
            => 31 - CountLeadingZeros(value);

        /// <summary>
        /// Returns the floored binary logarithm of the <b>absolute</b> value of the given value.
        /// <para>Note: <c>Log2(0)</c> returns <c>-1</c>.</para>
        /// </summary>
        /// <returns>The logarithm (range -1 to 31)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Log2(int value)
            => 31 - CountLeadingZeros((uint)Math.Abs(value));
    }
}