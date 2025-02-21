using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Progress;
using Fasciculus.Support.Progressing;
using Fasciculus.Threading.Synchronization;
using System;
using System.ComponentModel;

namespace Fasciculus.Maui.Support.Progressing
{
    /// <summary>
    /// An accumulating progress to use for progress bars.
    /// </summary>
    public partial class ProgressBarProgress : MainThreadObservable, IAccumulatingProgress<long>
    {
        private readonly ReentrantTaskSafeMutex mutex = new();

        private readonly ObservableLongProgress progress;

        /// <summary>
        /// The current progress with value range of <c>0.0</c> to <c>1.0</c> in steps of <c>0.01</c>.
        /// </summary>
        [ObservableProperty]
        public partial double Value { get; private set; }

        /// <summary>
        /// Whether the progressing activity has ended.
        /// </summary>
        [ObservableProperty]
        public partial bool Done { get; private set; }

        /// <summary>
        /// Initializes a new progress.
        /// </summary>
        public ProgressBarProgress()
        {
            progress = new ObservableLongProgress();
            progress.PropertyChanged += OnProgressChanged;
        }

        private void OnProgressChanged(object? sender, PropertyChangedEventArgs e)
            => UpdateProperties();

        private void UpdateProperties()
        {
            using Locker locker = Locker.Lock(mutex);

            double oldValue = Value;
            long current = progress.Current;
            long total = Math.Max(1, progress.Total);
            long percents = (current * 100) / total;
            double newValue = percents / 100.0;

            if (newValue != oldValue)
            {
                Value = newValue;
            }

            bool oldDone = Done;
            bool newDone = current == total;

            if (newDone != oldDone)
            {
                Done = newDone;
            }
        }

        /// <summary>
        /// Starts the progress with the given <paramref name="total"/> end value.
        /// </summary>
        public void Begin(long total)
            => progress.Begin(total);

        /// <summary>
        /// Ends the progress.
        /// </summary>
        public void End()
            => progress.End();

        /// <summary>
        /// Reports a progress update.
        /// </summary>
        public void Report(long value)
            => progress.Report(value);
    }
}
