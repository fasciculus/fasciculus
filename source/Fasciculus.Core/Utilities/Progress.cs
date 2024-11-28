using Fasciculus.Support;
using Fasciculus.Threading;
using System;
using System.Diagnostics;

namespace Fasciculus.Utilities
{
    public class TaskSafeProgress<T> : IProgress<T>
    {
        protected readonly ReentrantTaskSafeMutex mutex = new();

        protected Action<T>? report;

        public TaskSafeProgress(Action<T>? report = null)
        {
            this.report = report;
        }

        public virtual void Report(T value)
        {
            using Locker locker = Locker.Lock(mutex);

            Cond.NotNull(report)(value);
        }
    }

    public interface IAccumulatingProgress<T> : IProgress<T>
    {
        public T Total { get; }
        public T Current { get; }

        public void Begin(T total);
        public void End();
    }

    public class AccumulatingProgress<T> : TaskSafeProgress<T>, IAccumulatingProgress<T>
        where T : notnull
    {
        private readonly Func<T, T, T>? accumulate;
        private readonly T start;

        private readonly long interval;
        private readonly Stopwatch? stopwatch;

        private long ElapsedTime => stopwatch?.ElapsedMilliseconds ?? 0;

        public T Total { get; private set; }
        public T Current { get; private set; }

        public AccumulatingProgress(Action<T>? report, Func<T, T, T>? accumulate, T total, T start, long interval = 0)
            : base(report)
        {
            this.accumulate = accumulate;
            this.start = start;

            this.interval = interval;
            stopwatch = interval > 0 ? new() : null;

            Total = total;
            Current = start;
        }

        public void Begin(T total)
        {
            using Locker locker = Locker.Lock(mutex);

            Total = total;
            Current = start;

            stopwatch?.Restart();

            DoReport(true);
        }

        public void End()
        {
            using Locker locker = Locker.Lock(mutex);

            stopwatch?.Stop();

            DoReport(true);
        }

        public override void Report(T value)
        {
            using Locker locker = Locker.Lock(mutex);

            Current = Accumulate(Current, value);

            DoReport(false);
        }

        private void DoReport(bool forced)
        {
            if (forced)
            {
                base.Report(Current);
            }

            if (ElapsedTime >= interval)
            {
                stopwatch?.Restart();

                base.Report(Current);
            }
        }

        protected virtual T Accumulate(T current, T value)
        {
            return Cond.NotNull(accumulate)(current, value);
        }
    }

    public interface IAccumulatingLongProgress : IAccumulatingProgress<long>
    {
        public double Progress { get; }
    }

    public class AccumulatingLongProgress : AccumulatingProgress<long>, IAccumulatingLongProgress
    {
        public double Progress
        {
            get
            {
                double progress = 1;

                if (Total > 0 && Current < Total)
                {
                    progress *= Current;
                    progress /= Total;
                }

                return progress;
            }
        }

        public AccumulatingLongProgress(Action<long>? report, long interval = 0)
            : base(report, null, 0, 0, interval)
        {
        }

        protected override long Accumulate(long current, long value)
            => current + value;
    }
}
