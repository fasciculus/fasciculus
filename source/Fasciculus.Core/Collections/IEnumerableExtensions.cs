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

#if NETSTANDARD
        /// <summary>
        /// Adds <c>ToDictionary</c> for <see cref="KeyValuePair{TKey, TValue}"/> collection missing in netstandard.
        /// </summary>
        public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<KeyValuePair<K, V>> kvps)
            where K : notnull
            => kvps.ToDictionary(x => x.Key, x => x.Value);
#endif

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
    }
}
