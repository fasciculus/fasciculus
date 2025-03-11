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
        private readonly Func<T>? factory;
        private readonly Func<Task<T>>? factoryTask;

        private readonly AsyncMutex mutex = new();
        private Task<T>? value;

        /// <summary>
        /// Gets the lazily initialized value of the current instance.
        /// </summary>
        public Task<T> Value => GetValue();

        /// <summary>
        /// Initializes a new lazy instance with the given <paramref name="factory"/>.
        /// </summary>
        public AsyncLazy(Func<T> factory)
        {
            this.factory = factory;
        }

        /// <summary>
        /// Initializes a new lazy instance with the given <paramref name="factoryTask"/>.
        /// </summary>
        public AsyncLazy(Func<Task<T>> factoryTask)
        {
            this.factoryTask = factoryTask;
        }

        private Task<T> GetValue()
        {
            using AsyncLock asyncLock = Tasks.Wait(AsyncLock.Lock(mutex));

            return value ??= CreateValue();
        }

        private Task<T> CreateValue()
        {
            if (factory is not null)
            {
                return Tasks.Start(factory);
            }

            if (factoryTask is not null)
            {
                return Tasks.Start(factoryTask).Unwrap();
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Returns the awaiter of this instances value.
        /// </summary>
        public TaskAwaiter<T> GetAwaiter()
            => Value.GetAwaiter();
    }
}
