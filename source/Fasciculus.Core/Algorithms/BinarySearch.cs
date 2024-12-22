using System;

namespace Fasciculus.Algorithms
{
    /// <summary>
    /// Fast binary search implementation for base types.
    /// </summary>
    public static class BinarySearch
    {
        /// <summary>
        /// Searches the position of an occurrance of the given value in the given <b>sorted</b> array.
        /// </summary>
        /// <returns>The index within the array at which <c>value</c> occurs or <c>-1</c> if not found.</returns>
        public static unsafe int IndexOf(ReadOnlySpan<int> sorted, int value)
        {
            fixed (int* a = sorted)
            {
                return IndexOf(a, sorted.Length, value);
            }
        }

        /// <summary>
        /// Searches the position of an occurrance of the given value in the given <b>sorted</b> array.
        /// </summary>
        /// <returns>The index within the array at which <c>value</c> occurs or <c>-1</c> if not found.</returns>
        public static unsafe int IndexOf(ReadOnlySpan<uint> sorted, uint value)
        {
            fixed (uint* a = sorted)
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

        internal static unsafe int IndexOf(uint* pa, int na, uint value)
        {
            int lo = 0;
            int hi = na - 1;

            while (lo <= hi)
            {
                int i = lo + ((hi - lo) >> 1);
                uint x = pa[i];

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
