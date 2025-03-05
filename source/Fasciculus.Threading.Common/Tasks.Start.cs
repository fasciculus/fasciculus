using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fasciculus.Threading
{
    /// <summary>
    /// Utilities for <see cref="Task"/>s.
    /// </summary>
    public static partial class Tasks
    {
        /// <summary>
        /// Returns task creation options according to the specified <paramref name="longRunning"/> value.
        /// </summary>
        private static TaskCreationOptions GetTaskCreationOptions(bool longRunning)
            => longRunning ? TaskCreationOptions.LongRunning : TaskCreationOptions.None;

        /// <summary>
        /// Creates and starts a new task.
        /// </summary>
        public static Task Start(Action action, CancellationToken cancellationToken, TaskCreationOptions options, TaskScheduler scheduler)
            => Task.Factory.StartNew(action, cancellationToken, options, scheduler);

        /// <summary>
        /// Creates and starts a new task.
        /// </summary>
        public static Task Start(Action action, CancellationToken cancellationToken, bool longRunning, TaskScheduler scheduler)
            => Task.Factory.StartNew(action, cancellationToken, GetTaskCreationOptions(longRunning), scheduler);

        /// <summary>
        /// Creates and starts a new task.
        /// </summary>
        public static Task Start(Action action, CancellationToken cancellationToken, TaskCreationOptions options)
            => Task.Factory.StartNew(action, cancellationToken, options, Task.Factory.Scheduler);

        /// <summary>
        /// Creates and starts a new task.
        /// </summary>
        public static Task Start(Action action, CancellationToken cancellationToken, bool longRunning)
            => Task.Factory.StartNew(action, cancellationToken, GetTaskCreationOptions(longRunning), Task.Factory.Scheduler);

        /// <summary>
        /// Creates and starts a new task.
        /// </summary>
        public static Task Start(Action action, CancellationToken cancellationToken, TaskScheduler scheduler)
            => Task.Factory.StartNew(action, cancellationToken, TaskCreationOptions.None, scheduler);

        /// <summary>
        /// Creates and starts a new task.
        /// </summary>
        public static Task Start(Action action, TaskCreationOptions options, TaskScheduler scheduler)
            => Task.Factory.StartNew(action, CancellationToken.None, options, scheduler);

        /// <summary>
        /// Creates and starts a new task.
        /// </summary>
        public static Task Start(Action action, bool longRunning, TaskScheduler scheduler)
            => Task.Factory.StartNew(action, CancellationToken.None, GetTaskCreationOptions(longRunning), scheduler);

        /// <summary>
        /// Creates and starts a new task.
        /// </summary>
        public static Task Start(Action action, CancellationToken cancellationToken)
            => Task.Factory.StartNew(action, cancellationToken, TaskCreationOptions.None, Task.Factory.Scheduler);

        /// <summary>
        /// Creates and starts a new task.
        /// </summary>
        public static Task Start(Action action, TaskCreationOptions options)
            => Task.Factory.StartNew(action, CancellationToken.None, options, Task.Factory.Scheduler);

        /// <summary>
        /// Creates and starts a new task.
        /// </summary>
        public static Task Start(Action action, bool longRunning)
            => Task.Factory.StartNew(action, CancellationToken.None, GetTaskCreationOptions(longRunning), Task.Factory.Scheduler);

        /// <summary>
        /// Creates and starts a new task.
        /// </summary>
        public static Task Start(Action action, TaskScheduler scheduler)
            => Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, scheduler);

        /// <summary>
        /// Creates and starts a new task.
        /// </summary>
        public static Task Start(Action action)
            => Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, Task.Factory.Scheduler);

        /// <summary>
        /// Creates and starts a new task.
        /// </summary>
        public static Task<T> Start<T>(Func<T> func, CancellationToken cancellationToken, TaskCreationOptions options, TaskScheduler scheduler)
            => Task.Factory.StartNew(func, cancellationToken, options, scheduler);

        /// <summary>
        /// Creates and starts a new task.
        /// </summary>
        public static Task<T> Start<T>(Func<T> func, CancellationToken cancellationToken, bool longRunning, TaskScheduler scheduler)
            => Task.Factory.StartNew(func, cancellationToken, GetTaskCreationOptions(longRunning), scheduler);

        /// <summary>
        /// Creates and starts a new task.
        /// </summary>
        public static Task<T> Start<T>(Func<T> func, CancellationToken cancellationToken, TaskCreationOptions options)
            => Task.Factory.StartNew(func, cancellationToken, options, Task.Factory.Scheduler);

        /// <summary>
        /// Creates and starts a new task.
        /// </summary>
        public static Task<T> Start<T>(Func<T> func, CancellationToken cancellationToken, bool longRunning)
            => Task.Factory.StartNew(func, cancellationToken, GetTaskCreationOptions(longRunning), Task.Factory.Scheduler);

        /// <summary>
        /// Creates and starts a new task.
        /// </summary>
        public static Task<T> Start<T>(Func<T> func, CancellationToken cancellationToken, TaskScheduler scheduler)
            => Task.Factory.StartNew(func, cancellationToken, TaskCreationOptions.None, scheduler);

        /// <summary>
        /// Creates and starts a new task.
        /// </summary>
        public static Task<T> Start<T>(Func<T> func, TaskCreationOptions options, TaskScheduler scheduler)
            => Task.Factory.StartNew(func, CancellationToken.None, options, scheduler);

        /// <summary>
        /// Creates and starts a new task.
        /// </summary>
        public static Task<T> Start<T>(Func<T> func, bool longRunning, TaskScheduler scheduler)
            => Task.Factory.StartNew(func, CancellationToken.None, GetTaskCreationOptions(longRunning), scheduler);

        /// <summary>
        /// Creates and starts a new task.
        /// </summary>
        public static Task<T> Start<T>(Func<T> func, CancellationToken cancellationToken)
            => Task.Factory.StartNew(func, cancellationToken, TaskCreationOptions.None, Task.Factory.Scheduler);

        /// <summary>
        /// Creates and starts a new task.
        /// </summary>
        public static Task<T> Start<T>(Func<T> func, TaskCreationOptions options)
            => Task.Factory.StartNew(func, CancellationToken.None, options, Task.Factory.Scheduler);

        /// <summary>
        /// Creates and starts a new task.
        /// </summary>
        public static Task<T> Start<T>(Func<T> func, bool longRunning)
            => Task.Factory.StartNew(func, CancellationToken.None, GetTaskCreationOptions(longRunning), Task.Factory.Scheduler);

        /// <summary>
        /// Creates and starts a new task.
        /// </summary>
        public static Task<T> Start<T>(Func<T> func, TaskScheduler scheduler)
            => Task.Factory.StartNew(func, CancellationToken.None, TaskCreationOptions.None, scheduler);

        /// <summary>
        /// Creates and starts a new task.
        /// </summary>
        public static Task<T> Start<T>(Func<T> func)
            => Task.Factory.StartNew(func, CancellationToken.None, TaskCreationOptions.None, Task.Factory.Scheduler);

    }
}