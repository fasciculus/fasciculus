using System;
using System.Threading.Tasks;

namespace Fasciculus.Threading
{
    /// <summary>
    /// Factory creating a task for an func.
    /// </summary>
    public interface IAsyncFunc<T>
    {
        /// <summary>
        /// Creates a task for the stored func.
        /// </summary>
        public Task<T> Create();
    }

    /// <summary>
    /// Factory creating a task for a func.
    /// </summary>
    public class AsyncFunc<T> : IAsyncFunc<T>
    {
        private readonly Func<T> func;

        /// <summary>
        /// Initializes this factory.
        /// </summary>
        public AsyncFunc(Func<T> func)
        {
            this.func = func;
        }

        /// <summary>
        /// Creates a task for the stored function.
        /// </summary>
        public Task<T> Create()
        {
            return Tasks.Start(func);
        }
    }

    /// <summary>
    /// Factory creating a task for an async function.
    /// </summary>
    public class AsyncWrappedFunc<T> : IAsyncFunc<T>
    {
        private readonly Func<Task<T>> asyncFunc;

        /// <summary>
        /// Initializes this factory.
        /// </summary>
        public AsyncWrappedFunc(Func<Task<T>> asyncFunc)
        {
            this.asyncFunc = asyncFunc;
        }

        /// <summary>
        /// Creates a task for the stored async function.
        /// </summary>
        public Task<T> Create()
        {
            return Tasks.Start(asyncFunc).Unwrap();
        }
    }
}