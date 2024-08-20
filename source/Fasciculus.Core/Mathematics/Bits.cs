using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Mathematics
{
    public static class Bits
    {
        public static int Count(byte value)
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

        public static int Count(IEnumerable<byte> values)
            => values.Select(Count).Sum();

        public static IEnumerator<int> Indices(byte value)
        {
            byte mask = 1;

            for (int i = 0; i < 8; ++i, mask <<= 1)
            {
                if ((value & mask) != 0)
                {
                    yield return i;
                }
            }
        }

        public static byte IndicesToByte(IEnumerable<int> indices)
        {
            byte result = 0;
            byte mask = 1;

            foreach (int i in indices)
            {
                result |= (byte)(mask << i);
            }

            return result;
        }
    }
}