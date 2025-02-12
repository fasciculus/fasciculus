using System;

namespace Fasciculus.Algorithms
{
    /// <summary>
    /// Fast implementations to check equality of base type arrays.
    /// </summary>
    public static class Equality
    {
        /// <summary>
        /// Returns whether the given arrays have the same size and contain the same values.
        /// </summary>
        public static bool AreEqual(ReadOnlySpan<byte> a, ReadOnlySpan<byte> b)
        {
            int na = a.Length;
            int nb = b.Length;

            if (na != nb)
            {
                return false;
            }

            for (int i = 0; i < na; ++i)
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
