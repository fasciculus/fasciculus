using System;
using System.Collections.Generic;

namespace Fasciculus.Algorithms
{
    /// <summary>
    /// A comparer to compare byte arrays.
    /// </summary>
    public class ByteArrayComparer : IComparer<byte[]>
    {
        /// <summary>
        /// The instance of this comparer.
        /// </summary>
        public static ByteArrayComparer Instance { get; } = new();

        private ByteArrayComparer() { }

        /// <summary>
        /// Compares the given byte arrays.
        /// </summary>
        public int Compare(byte[]? x, byte[]? y)
        {
            return Compare(x.AsSpan(), y.AsSpan());
        }

        /// <summary>
        /// Compares the given byte arrays.
        /// </summary>
        public static int Compare(ReadOnlySpan<byte> a, ReadOnlySpan<byte> b)
        {
            int na = a.Length;
            int nb = b.Length;
            int n = Math.Min(na, nb);

            for (int i = 0; i < n; ++i)
            {
                if (a[i] < b[i])
                {
                    return -1;
                }

                if (a[i] > b[i])
                {
                    return 1;
                }
            }

            if (na < nb)
            {
                return -1;
            }

            if (na > nb)
            {
                return 1;
            }

            return 0;
        }
    }
}
