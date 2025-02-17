using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Collections
{
    /// <summary>
    /// Extensions for <see cref="IEnumerable{T}"/>
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Returns an enumeration with those entries of the given <paramref name="values"/> that are not <c>null</c>.
        /// </summary>
        public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> values)
            where T : notnull
            => values.Where(x => x is not null).Cast<T>();

        /// <summary>
        /// A "ForEach" for all <see cref="IEnumerable{T}"/>.
        /// </summary>
        public static void Apply<T>(this IEnumerable<T> values, Action<T> action)
        {
            foreach (var value in values)
            {
                action(value);
            }
        }

        /// <summary>
        /// Returns exactly <paramref name="count"/> values, filling up with <paramref name="defaultValue"/> if the
        /// <paramref name="values"/> sequence has not enough values.
        /// </summary>
        public static IEnumerable<T> Take<T>(this IEnumerable<T> values, int count, T defaultValue)
        {
            IEnumerable<T> existing = values.Take(count);
            int existingCount = existing.Count();
            int missingCount = Math.Max(0, count - existingCount);

            return existing.Concat(Enumerable.Repeat(defaultValue, missingCount));
        }

        /// <summary>
        /// Converts the given key/value pairs into a dictionary.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="kvps"></param>
        /// <returns></returns>
        public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<Tuple<K, V>> kvps)
            where K : notnull
            => kvps.ToDictionary(x => x.Item1, x => x.Item2);

        /// <summary>
        /// Returns those <paramref name="values"/> whose type's full name equals to <paramref name="fullName"/>.
        /// </summary>
        public static IEnumerable<T> OfType<T>(this IEnumerable<T> values, string fullName)
            where T : notnull
            => values.Where(x => fullName == x.GetType().FullName);

        /// <summary>
        /// Accumulates the hash codes of the given values into a single hash code.
        /// </summary>
        public static int ToHashCode<T>(this IEnumerable<T> values)
            where T : notnull
            => values.Aggregate(0, (acc, val) => acc ^= val.GetHashCode());

        /// <summary>
        /// Compares the given sequences.
        /// </summary>
        public static int SequenceCompare<T>(this IEnumerable<T> values, IEnumerable<T> others, IComparer<T> comparer)
            where T : notnull
        {
            IEnumerator<T> lhs = values.GetEnumerator();
            IEnumerator<T> rhs = others.GetEnumerator();
            bool lhsHasNext = lhs.MoveNext();
            bool rhsHasNext = rhs.MoveNext();

            while (lhsHasNext && rhsHasNext)
            {
                int result = comparer.Compare(lhs.Current, rhs.Current);

                if (result != 0)
                {
                    return result;
                }

                lhsHasNext = lhs.MoveNext();
                rhsHasNext = rhs.MoveNext();
            }

            return lhsHasNext ? 1 : (rhsHasNext ? -1 : 0);
        }
    }
}

