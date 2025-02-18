using System.Collections.Generic;

namespace System.Linq
{
    internal static partial class FasciculusEnumerableExtensions
    {
        internal static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source)
            where TKey : notnull
        {
            return source.ToDictionary(null);
        }

        internal static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source,
            IEqualityComparer<TKey>? comparer)
            where TKey : notnull
        {
            comparer ??= EqualityComparer<TKey>.Default;

            return source.ToDictionary(kvp => kvp.Key, kvp => kvp.Value, comparer);
        }
    }
}