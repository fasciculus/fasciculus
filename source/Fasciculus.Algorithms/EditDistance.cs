using System;
using System.Collections.Generic;

namespace Fasciculus.Algorithms
{
    /// <summary>
    /// Calculates the edit distance between two sequences.
    /// </summary>
    public static class EditDistance
    {
        /// <summary>
        /// Returns the edit distance between the given sequences <paramref name="a"/> and <paramref name="b"/>
        /// using the given <paramref name="comparer"/>.
        /// </summary>
        public static int GetDistance<T>(ReadOnlySpan<T> a, ReadOnlySpan<T> b, IEqualityComparer<T> comparer)
        {
            int na = a.Length;
            int nb = b.Length;
            int[] curr = new int[nb + 1];
            int prev;

            for (int j = 0; j <= nb; ++j)
            {
                curr[j] = j;
            }

            for (int i = 1; i <= na; ++i)
            {
                prev = curr[0];
                curr[0] = i;

                for (int j = 1; j <= nb; ++j)
                {
                    int temp = curr[j];

                    if (comparer.Equals(a[i - 1], b[j - 1]))
                    {
                        curr[j] = prev;
                    }
                    else
                    {
                        curr[j] = 1 + Math.Min(Math.Min(curr[j - 1], prev), curr[j]);
                    }

                    prev = temp;
                }
            }

            return curr[nb];
        }

        /// <summary>
        /// Returns the edit distance between the given sequences <paramref name="a"/> and <paramref name="b"/>
        /// using the default comparer for the given type <typeparamref name="T"/>.
        /// </summary>
        public static int GetDistance<T>(ReadOnlySpan<T> a, ReadOnlySpan<T> b)
            => GetDistance(a, b, EqualityComparer<T>.Default);

        /// <summary>
        /// Returns the edit distance between the given sequences <paramref name="a"/> and <paramref name="b"/>
        /// using the given <paramref name="comparer"/>.
        /// </summary>
        public static int GetDistance<T>(IEnumerable<T> a, IEnumerable<T> b, IEqualityComparer<T> comparer)
            => GetDistance([.. a], [.. b], comparer);

        /// <summary>
        /// Returns the edit distance between the given sequences <paramref name="a"/> and <paramref name="b"/>
        /// using the default comparer for the given type <typeparamref name="T"/>.
        /// </summary>
        public static int GetDistance<T>(IEnumerable<T> a, IEnumerable<T> b)
            => GetDistance(a, b, EqualityComparer<T>.Default);
    }
}
