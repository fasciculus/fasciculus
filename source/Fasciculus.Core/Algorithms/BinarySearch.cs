using System;
using System.Runtime.CompilerServices;

namespace Fasciculus.Algorithms
{
    public static class BinarySearch
    {
        public static int IndexOf(ReadOnlySpan<int> sorted, int value)
        {
            int lo = 0;
            int hi = sorted.Length - 1;

            while (lo <= hi)
            {
                int i = lo + ((hi - lo) >> 1);
                int x = sorted[i];

                if (x == value)
                {
                    return i;
                }

                if (x < value)
                {
                    lo = i + 1;
                }
                else
                {
                    hi = i - 1;
                }
            }

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf(int[] sorted, int start, int count, int value)
            => IndexOf(new ReadOnlySpan<int>(sorted, start, count), value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf(int[] sorted, int value)
            => IndexOf(new ReadOnlySpan<int>(sorted, 0, sorted.Length), value);
    }
}
