using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Fasciculus.Algorithms
{
    /// <summary>
    /// Equality comparer for byte arrays.
    /// </summary>
    public class ByteArrayEqualityComparer : IEqualityComparer<byte[]>
    {
        /// <summary>
        /// The instance of this comparer.
        /// </summary>
        public static ByteArrayEqualityComparer Instance { get; } = new();

        /// <summary>
        /// Returns whether the two arrays are equal.
        /// </summary>
        public bool Equals(byte[]? x, byte[]? y)
        {
            return AreEqual(x.AsSpan(), y.AsSpan());
        }

        /// <summary>
        /// Returns whether the two arrays are equal.
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

        /// <summary>
        /// Returns a hash code for the given array. 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
#if NETSTANDARD
        public int GetHashCode(byte[] obj)
#else
        public int GetHashCode([DisallowNull] byte[] obj)
#endif
        {
            int result = 0;

            for (int i = 0, n = obj.Length; i < n; ++i)
            {
                result ^= obj[i];
            }

            return result;
        }
    }
}
