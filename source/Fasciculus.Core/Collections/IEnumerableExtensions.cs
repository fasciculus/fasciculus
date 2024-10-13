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
    }
}
