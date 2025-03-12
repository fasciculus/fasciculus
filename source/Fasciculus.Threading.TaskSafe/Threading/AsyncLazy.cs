using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Fasciculus.Threading
{
    /// <summary>
    /// Task-safe async lazy.
    /// </summary>
    public class AsyncLazy<T>
        where T : notnull
    {
        private readonly AsyncMutex mutex = new();
        private readonly IAsyncFunc<T> factory;
        private Task<T>? value = null;

        /// <summary>
        /// Gets the lazily initialized value of the current instance.
        /// </summary>
        public Task<T> Value => GetValue();

        /// <summary>
        /// Initializes a new lazy instance with the given <paramref name="factory"/>.
        /// </summary>
        public AsyncLazy(Func<T> factory)
        {
            this.factory = AsyncFactory.Create(factory);
        }

        /// <summary>
        /// Initializes a new lazy instance with the given <paramref name="factory"/>.
        /// </summary>
        public AsyncLazy(Func<Task<T>> factory)
        {
            this.factory = AsyncFactory.Create(factory);
        }

        private Task<T> GetValue()
        {
            using AsyncLock asyncLock = AsyncLock.Lock(mutex);

            value ??= factory.Create();

            return value;
        }

        /// <summary>
        /// Returns the awaiter of this instances value.
        /// </summary>
        public TaskAwaiter<T> GetAwaiter()
            => Value.GetAwaiter();
    }
}
