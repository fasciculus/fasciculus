using System;
using System.Threading.Tasks;

namespace Fasciculus.Threading
{
    /// <summary>
    /// Helper to ctreate async factories.
    /// </summary>
    public static class AsyncFactory
    {
        /// <summary>
        /// Creates a factory to create tasks for the given <paramref name="action"/>.
        /// </summary>
        public static IAsyncAction Create(Action action)
            => new AsyncAction(action);

        /// <summary>
        /// Creates a factory to create tasks for the given <paramref name="asyncAction"/>.
        /// </summary>
        public static IAsyncAction Create(Func<Task> asyncAction)
            => new AsyncWrappedAction(asyncAction);

        /// <summary>
        /// Creates a factory to create tasks for the given <paramref name="func"/>.
        /// </summary>
        public static IAsyncFunc<T> Create<T>(Func<T> func)
            => new AsyncFunc<T>(func);

        /// <summary>
        /// Creates a factory to create tasks for the given <paramref name="asyncFunc"/>.
        /// </summary>
        public static IAsyncFunc<T> Create<T>(Func<Task<T>> asyncFunc)
            => new AsyncWrappedFunc<T>(asyncFunc);
    }
}