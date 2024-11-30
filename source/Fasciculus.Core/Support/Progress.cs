using Fasciculus.Threading;
using System;
using System.Diagnostics;

namespace Fasciculus.Support
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
        public T Current { get; protected set; }

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

        public virtual void End()
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
            else if (ElapsedTime >= interval)
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

    public class LongProgressInfo : IEquatable<LongProgressInfo>, IComparable<LongProgressInfo>
    {
        public bool Done { get; }
        public double Value { get; }

        public LongProgressInfo(bool done, double value)
        {
            Done = done;
            Value = value;
        }

        public bool Equals(LongProgressInfo other)
            => Done == other.Done && Value == other.Value;

        public int CompareTo(LongProgressInfo other)
        {
            int result = Done.CompareTo(other.Done);

            if (result == 0)
            {
                result = Value.CompareTo(other.Value);
            }

            return result;
        }

        public override bool Equals(object obj)
            => obj is LongProgressInfo other && Equals(other);

        public override int GetHashCode()
            => Done.GetHashCode() + Value.GetHashCode();

        public static bool operator ==(LongProgressInfo lhs, LongProgressInfo rhs) => lhs.Equals(rhs);
        public static bool operator !=(LongProgressInfo lhs, LongProgressInfo rhs) => !lhs.Equals(rhs);

        public static bool operator <(LongProgressInfo lhs, LongProgressInfo rhs) => lhs.CompareTo(rhs) < 0;
        public static bool operator >(LongProgressInfo lhs, LongProgressInfo rhs) => lhs.CompareTo(rhs) > 0;

        public static bool operator <=(LongProgressInfo lhs, LongProgressInfo rhs) => lhs.CompareTo(rhs) <= 0;
        public static bool operator >=(LongProgressInfo lhs, LongProgressInfo rhs) => lhs.CompareTo(rhs) >= 0;

        public static LongProgressInfo Start => new(false, 0);
    }

    public interface IAccumulatingLongProgress : IAccumulatingProgress<long>
    {
        public LongProgressInfo Progress { get; }
    }

    public class AccumulatingLongProgress : AccumulatingProgress<long>, IAccumulatingLongProgress
    {
        public bool Done => Locker.Locked(mutex, () => Current == Total);

        public LongProgressInfo Progress
        {
            get
            {
                using Locker locker = Locker.Lock(mutex);

                double value = 1;

                if (Total > 0 && Current < Total)
                {
                    value *= Current;
                    value /= Total;
                }

                return new(Current == Total, value);
            }
        }

        public AccumulatingLongProgress(Action<long>? report, long interval = 0)
            : base(report, null, 0, 0, interval)
        {
        }

        protected override long Accumulate(long current, long value)
            => current + value;

        public override void End()
        {
            using Locker locker = Locker.Lock(mutex);

            Current = Total;

            base.End();
        }
    }
}
