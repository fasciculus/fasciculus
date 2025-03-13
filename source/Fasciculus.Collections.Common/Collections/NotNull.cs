using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Collections
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Returns an enumeration with those entries of the given <paramref name="values"/> that are not <c>null</c>.
        /// </summary>
        public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> values)
            where T : notnull
            => values.Where(x => x is not null).Cast<T>();
    }
}