using Fasciculus.Threading;
using System;

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

        private long stepSize;
        private long count;

        public T Total { get; private set; }
        public T Current { get; protected set; }

        public AccumulatingProgress(Action<T>? report, Func<T, T, T>? accumulate, T total, T start)
            : base(report)
        {
            this.accumulate = accumulate;
            this.start = start;

            stepSize = 1;

            Total = total;
            Current = start;
        }

        protected virtual long GetStepSize()
            => 1;

        public void Begin(T total)
        {
            using Locker locker = Locker.Lock(mutex);

            Total = total;
            Current = start;

            stepSize = GetStepSize();
            count = 0;

            DoReport(true);
        }

        public virtual void End()
        {
            using Locker locker = Locker.Lock(mutex);

            DoReport(true);
        }

        public override void Report(T value)
        {
            using Locker locker = Locker.Lock(mutex);

            Current = Accumulate(Current, value);
            ++count;

            DoReport(false);
        }

        private void DoReport(bool forced)
        {
            if (forced)
            {
                base.Report(Current);
            }
            else if ((count % stepSize) == 0)
            {
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

        public bool Equals(LongProgressInfo? other)
            => other is not null && Done == other.Done && Value == other.Value;

        public int CompareTo(LongProgressInfo? other)
        {
            if (other is null) return -1;

            int result = Done.CompareTo(other.Done);

            if (result == 0)
            {
                result = Value.CompareTo(other.Value);
            }

            return result;
        }

        public override bool Equals(object? obj)
            => obj is not null && obj is LongProgressInfo other && Equals(other);

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

        public AccumulatingLongProgress(Action<long>? report)
            : base(report, null, 0, 0)
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

        protected override long GetStepSize()
        {
            return Math.Max(1, Total / 100);
        }
    }
}
