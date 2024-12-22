using System;

namespace Fasciculus.Algorithms
{
    /// <summary>
    /// Fast implementation to check equality of base type arrays.
    /// </summary>
    public static class Equality
    {
        /// <summary>
        /// Returns whether the given arrays have the same size and contain the same values.
        /// </summary>
        /// <param name="a">The first array.</param>
        /// <param name="b">The second array.</param>
        /// <returns>Whether the two arrays are identical.</returns>
        public static unsafe bool AreEqual(ReadOnlySpan<byte> a, ReadOnlySpan<byte> b)
        {
            int na = a.Length;
            int nb = b.Length;

            if (na != nb)
            {
                return false;
            }

            fixed (byte* pa = a, pb = b)
            {
                return AreEqual(pa, pb, na);
            }
        }

        private static unsafe bool AreEqual(byte* a, byte* b, int n)
        {
            for (int i = 0; i < n; ++i)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
