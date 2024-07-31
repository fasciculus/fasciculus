namespace System.Collections.Generic
{
    public static class IEnumerableExtensions
    {
        public static void Apply<T>(this IEnumerable<T> values, Action<T> action)
        {
            foreach (var value in values)
            {
                action(value);
            }
        }
    }
}
