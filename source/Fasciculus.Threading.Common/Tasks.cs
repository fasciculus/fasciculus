using System;
using System.Threading.Tasks;

namespace Fasciculus.Threading
{
    /// <summary>
    /// Utilities for <see cref="Task"/>s.
    /// </summary>
    public static partial class Tasks
    {
        /// <summary>
        /// Starts a new <see cref="Task"/> for the given <paramref name="func"/>.
        /// <para>Shorthand for <c>Task.Factory.StartNew(func)</c></para>
        /// </summary>
        public static Task<T> Start<T>(Func<T> func)
            => Task<T>.Factory.StartNew(func);

        /// <summary>
        /// Starts a new long-running <see cref="Task"/> for the given <paramref name="action"/>.
        /// <para>Shorthand for <c>Task.Factory.StartNew(action, TaskCreationOptions.LongRunning)</c></para>
        /// </summary>
        public static Task LongRunning(Action action)
            => Task.Factory.StartNew(action, TaskCreationOptions.LongRunning);

        /// <summary>
        /// Starts a new long-running <see cref="Task"/> for the given <paramref name="func"/>.
        /// <para>Shorthand for <c>Task.Factory.StartNew(func, TaskCreationOptions.LongRunning)</c></para>
        /// </summary>
        public static Task<T> LongRunning<T>(Func<T> func)
            => Task.Factory.StartNew(func, TaskCreationOptions.LongRunning);

        /// <summary>
        /// Synchronously waits for the given <paramref name="task"/> to finish and returns its result.
        /// </summary>
        public static T Wait<T>(Task<T> task)
        {
            Task.WaitAll([task]);

            return task.Result;
        }
    }
}
