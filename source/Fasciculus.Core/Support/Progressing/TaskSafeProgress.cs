using Fasciculus.Threading.Synchronization;
using System;

namespace Fasciculus.Support.Progressing
{
    /// <summary>
    /// A task-safe <see cref="IProgress{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TaskSafeProgress<T> : IProgress<T>
    {
        /// <summary>
        /// The protecting mutex. Available to derived classes.
        /// </summary>
        protected readonly ReentrantTaskSafeMutex mutex = new();

        /// <summary>
        /// The optional action that does the actual reporting.
        /// </summary>
        protected Action<T>? report;

        /// <summary>
        /// Initializes a new progress.
        /// <para>
        /// A non-null <paramref name="report"/> must be given unless the <see cref="Report(T)"/> method is overridden.
        /// </para>
        /// </summary>
        /// <param name="report"></param>
        public TaskSafeProgress(Action<T>? report = null)
        {
            this.report = report;
        }

        /// <summary>
        /// Reports a progress update.
        /// </summary>
        public virtual void Report(T value)
        {
            using Locker locker = Locker.Lock(mutex);

            Cond.NotNull(report)(value);
        }
    }
}
