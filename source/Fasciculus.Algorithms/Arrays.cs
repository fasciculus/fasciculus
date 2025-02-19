using System;
using System.Buffers;

namespace Fasciculus.Algorithms
{
    /// <summary>
    /// Array extensions.
    /// </summary>
    public static class Arrays
    {
        /// <summary>
        /// Invokes the specified action with an array of the specified minimum length.
        /// </summary>
        public static void Invoke<T>(ArrayPool<T> pool, int minimumLength, Action<T[]> action)
        {
            T[] array = pool.Rent(minimumLength);

            try
            {
                action(array);
            }
            finally
            {
                pool.Return(array);
            }
        }

        /// <summary>
        /// Invokes the specified action with an array of the specified minimum length.
        /// </summary>
        public static void Invoke<T>(int minimumLength, Action<T[]> action)
            => Invoke(ArrayPool<T>.Shared, minimumLength, action);

        /// <summary>
        /// Invokes the specified function with an array of the specified minimum length.
        /// </summary>
        public static R Select<T, R>(ArrayPool<T> pool, int minimumLength, Func<T[], R> func)
        {
            T[] array = pool.Rent(minimumLength);

            try
            {
                return func(array);
            }
            finally
            {
                pool.Return(array);
            }
        }

        /// <summary>
        /// Invokes the specified function with an array of the specified minimum length.
        /// </summary>
        public static R Select<T, R>(int minimumLength, Func<T[], R> func)
            => Select(ArrayPool<T>.Shared, minimumLength, func);
    }
}
