using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Progress;
using Fasciculus.Threading.Synchronization;
using System;

namespace Fasciculus.Support.Progressing
{
    /// <summary>
    /// Accumulating progress.
    /// </summary>
    public partial class ObservableAccumulatingProgress<T> : ObservableObject, IAccumulatingProgress<T>
        where T : notnull, IEquatable<T>
    {
        private readonly ReentrantTaskSafeMutex mutex = new();

        private readonly Func<T, T, T>? accumulate;

        private readonly T start;

        /// <summary>
        /// The currently accumulated value.
        /// </summary>
        [ObservableProperty]
        public partial T Current { get; private set; }

        /// <summary>
        /// The total value as set in <see cref="Begin(T)"/>.
        /// </summary>
        [ObservableProperty]
        public partial T Total { get; private set; }

        /// <summary>
        /// Initializes a new accumulating progress.
        /// <para>
        /// A non-null <paramref name="accumulate"/> must be given unless the <see cref="Accumulate(T)"/> method is overridden.
        /// </para>
        /// </summary>
        public ObservableAccumulatingProgress(Func<T, T, T>? accumulate, T start)
        {
            this.accumulate = accumulate;
            this.start = start;

            Current = start;
            Total = start;
        }

        /// <summary>
        /// Starts the progress with the given <paramref name="total"/> end value.
        /// </summary>
        public void Begin(T total)
        {
            using Locker locker = Locker.Lock(mutex);

            Current = start;
            Total = total;
        }

        /// <summary>
        /// Ends the progress.
        /// </summary>
        public void End()
            => Locker.Locked(mutex, () => { Current = Total; });

        /// <summary>
        /// Reports a progress update.
        /// </summary>
        public void Report(T value)
            => Locker.Locked(mutex, () => { Current = Accumulate(value); });

        /// <summary>
        /// Uses the accumulator given to the constructor to calculate the new <see cref="Current"/> value.
        /// <para>
        /// Override this method if a <c>null</c> accumulator is given to the constructor.
        /// </para>
        /// </summary>
        protected virtual T Accumulate(T value)
            => Cond.NotNull(accumulate)(Current, value);
    }
}
