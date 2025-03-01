using System.Collections.Generic;

namespace System.Linq
{
    internal static partial class FasciculusEnumerableExtensions
    {
        internal static IOrderedEnumerable<T> Order<T>(this IEnumerable<T> source)
            => source.Order(comparer: null);

        internal static IOrderedEnumerable<T> Order<T>(this IEnumerable<T> source, IComparer<T>? comparer)
        {
            comparer ??= Comparer<T>.Default;

            return source.OrderBy(x => x, comparer);
        }
    }
}