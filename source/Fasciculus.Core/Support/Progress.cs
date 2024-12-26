using Fasciculus.Support.Progressing;
using Fasciculus.Threading.Synchronization;
using System;

namespace Fasciculus.Support
{
    public class AccumulatingProgress<T> : TaskSafeProgress<T>, IAccumulatingProgress<T>
        where T : notnull
    {
        private readonly Func<T, T, T>? accumulate;
        private readonly T start;

        public T Total { get; private set; }
        public T Current { get; protected set; }

        public AccumulatingProgress(Action<T>? report, Func<T, T, T>? accumulate, T total, T start)
            : base(report)
        {
            this.accumulate = accumulate;
            this.start = start;

            Total = total;
            Current = start;
        }

        public void Begin(T total)
        {
            using Locker locker = Locker.Lock(mutex);

            Total = total;
            Current = start;

            DoReport();
        }

        public virtual void End()
        {
            using Locker locker = Locker.Lock(mutex);

            Current = Total;

            DoReport();
        }

        public sealed override void Report(T value)
        {
            using Locker locker = Locker.Lock(mutex);

            Current = Accumulate(Current, value);

            DoReport();
        }

        private void DoReport()
        {
            base.Report(Current);
        }

        protected virtual T Accumulate(T current, T value)
        {
            return Cond.NotNull(accumulate)(current, value);
        }
    }

    public class LongProgressInfo : IEquatable<LongProgressInfo>
    {
        public static LongProgressInfo Start => new(false, 0);

        public bool Done { get; }
        public double Value { get; }

        public LongProgressInfo(bool done, double value)
        {
            Done = done;
            Value = value;
        }

        public bool Equals(LongProgressInfo? other)
            => other is not null && Done == other.Done && Value == other.Value;

        public override bool Equals(object? obj)
            => obj is not null && obj is LongProgressInfo other && Equals(other);

        public override int GetHashCode()
            => Done.GetHashCode() + Value.GetHashCode();
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
    }
}
