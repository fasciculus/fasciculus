using System;
using System.Threading.Tasks;

namespace Fasciculus.Threading
{
    /// <summary>
    /// Utilities for <see cref="Task"/>s.
    /// </summary>
    public static class Tasks
    {
        /// <summary>
        /// Starts a new <see cref="Task"/> for the given <paramref name="action"/>.
        /// <para>Shorthand for <c>Task.Factory.StartNew(action)</c></para>
        /// </summary>
        public static Task Start(Action action)
            => Task.Factory.StartNew(action);

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
        /// Synchronously waits for the given <paramref name="task"/> to finish.
        /// <para>
        /// Shorthand for <c>task.GetAwaiter().GetResult()</c>.
        /// </para>
        /// </summary>
        public static T Wait<T>(Task<T> task)
            => task.GetAwaiter().GetResult();

        /// <summary>
        /// Waits for the given tasks to finish and returns their results.
        /// </summary>
        public static Tuple<T1, T2> Wait<T1, T2>(Task<T1> task1, Task<T2> task2)
        {
            Task.WaitAll([task1, task2]);

            return new(task1.Result, task2.Result);
        }

        /// <summary>
        /// Waits for the given tasks to finish and returns their results.
        /// </summary>
        public static Tuple<T1, T2, T3> Wait<T1, T2, T3>(Task<T1> task1, Task<T2> task2, Task<T3> task3)
        {
            Task.WaitAll([task1, task2, task3]);

            return new(task1.Result, task2.Result, task3.Result);
        }

        /// <summary>
        /// Waits for the given tasks to finish and returns their results.
        /// </summary>
        public static Tuple<T1, T2, T3, T4> Wait<T1, T2, T3, T4>(Task<T1> task1, Task<T2> task2, Task<T3> task3, Task<T4> task4)
        {
            Task.WaitAll([task1, task2, task3, task4]);

            return new(task1.Result, task2.Result, task3.Result, task4.Result);
        }
    }
}
