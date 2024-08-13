using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Mathematics
{
    public static class Bits
    {
        private static readonly int[] counts = GetCounts(0x10000);

        public static int Count(byte value)
            => counts[value];

        public static int Count(IEnumerable<byte> values)
            => values.Select(Count).Sum();

        public static int[] GetCounts(int length)
        {
            int[] counts = new int[length];

            for (int i = 0; i < length; ++i)
            {
                int count = 0;
                int x = i;

                while (x > 0)
                {
                    x &= x - 1;
                    ++count;
                }

                counts[i] = count;
            }

            return counts;
        }
    }
}
