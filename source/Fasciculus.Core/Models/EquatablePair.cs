using System;

namespace Fasciculus.Models
{
    public class EquatablePair<T1, T2> : Pair<T1, T2>, IEquatable<Pair<T1, T2>>
        where T1 : notnull, IEquatable<T1>
        where T2 : notnull, IEquatable<T2>
    {
        public EquatablePair(T1 first, T2 second)
            : base(first, second) { }

        public bool Equals(Pair<T1, T2>? other)
            => other is not null && First.Equals(other.First) && Second.Equals(other.Second);

        public override bool Equals(object? obj)
        {
            if (obj is not null && obj is Pair<T1, T2> other) return Equals(other);

            return false;
        }

        public override int GetHashCode()
            => base.GetHashCode();

        public static bool operator ==(EquatablePair<T1, T2> a, EquatablePair<T1, T2> b)
            => a.Equals(b);

        public static bool operator !=(EquatablePair<T1, T2> a, EquatablePair<T1, T2> b)
            => !a.Equals(b);
    }
}
