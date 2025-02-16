using System.Collections.Generic;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace System.Linq
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    internal static partial class FasciculusNetStandardEnumerableExtensions
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