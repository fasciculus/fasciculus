using System;
using System.Collections.Generic;

namespace Fasciculus.Algorithms
{
    public static class Bits
    {
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

        public static byte IndicesToByte(IEnumerable<ulong> indices)
        {
            byte result = 0;

            foreach (int i in indices)
            {
                result |= (byte)(1 << i);
            }

            return result;
        }

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

        public static int Log2(uint value)
            => 31 - CountLeadingZeros(value);

        public static int Log2(int value)
            => 31 - CountLeadingZeros((uint)Math.Abs(value));
    }
}