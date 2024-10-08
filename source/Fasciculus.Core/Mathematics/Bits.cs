﻿using System.Collections.Generic;

namespace Fasciculus.Mathematics
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
    }
}