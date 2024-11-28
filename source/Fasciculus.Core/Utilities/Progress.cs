using Fasciculus.Threading;
using System;
using System.Diagnostics;

namespace Fasciculus.Utilities
{
    public abstract class TaskSafeProgress<T> : IProgress<T>
    {
        private readonly TaskSafeMutex mutex = new();

        public void Report(T value)
            => Locker.Locked(mutex, () => OnReport(value));

        protected abstract void OnReport(T value);
    }

    public interface ILongProgress : IProgress<long>
    {
        public long Total { get; }
        public long Current { get; }

        public double Progress { get; }

        public void Start(long total);
        public void Done();
    }

    public abstract class LongProgress : TaskSafeProgress<long>, ILongProgress
    {
        private readonly TaskSafeMutex mutex = new();

        private long total;
        private long current;

        private long interval;
        private Stopwatch stopwatch = new();

        public long Total => total;
        public long Current => current;

        public double Progress
        {
            get
            {
                double progress = 1.0;

                if (total > 0 && current != total)
                {
                    progress *= current;
                    progress /= total;
                }

                return progress;
            }
        }

        protected LongProgress(long interval)
        {
            this.interval = interval;
        }

        public void Start(long total)
        {
            using Locker locker = Locker.Lock(mutex);

            this.total = total;
            current = 0;

            stopwatch.Restart();

            OnProgress();
        }

        public void Done()
        {
            using Locker locker = Locker.Lock(mutex);

            current = total;

            OnProgress();
        }

        protected override void OnReport(long value)
        {
            using Locker locker = Locker.Lock(mutex);

            current += value;

            if (stopwatch.ElapsedMilliseconds > interval)
            {
                stopwatch.Restart();
                OnProgress();
            }
        }

        protected abstract void OnProgress();
    }
}
