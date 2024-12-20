using System.Linq;

namespace System.Collections.Generic
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> collection)
            where T : notnull
            => collection.Where(x => x is not null).Cast<T>();

        public static void Apply<T>(this IEnumerable<T> values, Action<T> action)
        {
            foreach (var value in values)
            {
                action(value);
            }
        }

#if NETSTANDARD
        public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<KeyValuePair<K, V>> kvps)
            where K : notnull
            => kvps.ToDictionary(x => x.Key, x => x.Value);
#endif

        public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<Tuple<K, V>> kvps)
            where K : notnull
            => kvps.ToDictionary(x => x.Item1, x => x.Item2);
    }
}
