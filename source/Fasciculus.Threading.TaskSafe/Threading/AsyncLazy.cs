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
        /// <summary>
        /// Gets the lazily initialized value of the current instance.
        /// </summary>
        public Task<T> Value { get; }

        /// <summary>
        /// Initializes a new lazy instance with the given <paramref name="factory"/>.
        /// </summary>
        public AsyncLazy(Func<T> factory)
        {
            Value = Tasks.Start(factory);
        }

        /// <summary>
        /// Initializes a new lazy instance with the given <paramref name="factory"/>.
        /// </summary>
        public AsyncLazy(Func<Task<T>> factory)
        {
            Value = Tasks.Start(factory).Unwrap();
        }

        /// <summary>
        /// Returns the awaiter of this instances value.
        /// </summary>
        public TaskAwaiter<T> GetAwaiter()
            => Value.GetAwaiter();
    }
}
