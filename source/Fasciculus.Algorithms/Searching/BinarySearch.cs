using System;

namespace Fasciculus.Algorithms.Searching
{
    /// <summary>
    /// Fast binary search implementations for base types.
    /// </summary>
    public static class BinarySearch
    {
        /// <summary>
        /// Searches the position of an occurrance of the given value in the given <b>sorted</b> array.
        /// </summary>
        /// <returns>The index within the array at which <c>value</c> occurs or <c>-1</c> if not found.</returns>
        public static int IndexOf(ReadOnlySpan<int> sorted, int value)
        {
            int lo = 0;
            int hi = sorted.Length - 1;

            while (lo <= hi)
            {
                int i = lo + (hi - lo >> 1);
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

        /// <summary>
        /// Searches the position of an occurrance of the given value in the given <b>sorted</b> array.
        /// </summary>
        /// <returns>The index within the array at which <c>value</c> occurs or <c>-1</c> if not found.</returns>
        public static int IndexOf(ReadOnlySpan<uint> sorted, uint value)
        {
            int lo = 0;
            int hi = sorted.Length - 1;

            while (lo <= hi)
            {
                int i = lo + (hi - lo >> 1);
                uint x = sorted[i];

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
