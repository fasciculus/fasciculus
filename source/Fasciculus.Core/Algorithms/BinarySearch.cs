using System;

namespace Fasciculus.Algorithms
{
    public static class BinarySearch
    {
        public static unsafe int IndexOf(ReadOnlySpan<int> sorted, int value)
        {
            fixed (int* a = sorted)
            {
                return IndexOf(a, sorted.Length, value);
            }
        }

        internal static unsafe int IndexOf(int* pa, int na, int value)
        {
            int lo = 0;
            int hi = na - 1;

            while (lo <= hi)
            {
                int i = lo + ((hi - lo) >> 1);
                int x = pa[i];

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
    }
}
