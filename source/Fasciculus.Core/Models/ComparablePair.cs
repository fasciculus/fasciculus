using System;

namespace Fasciculus.Models
{
    public class ComparablePair<T1, T2> : EquatablePair<T1, T2>, IComparable<ComparablePair<T1, T2>>
        where T1 : notnull, IComparable<T1>, IEquatable<T1>
        where T2 : notnull, IComparable<T2>, IEquatable<T2>
    {
        public ComparablePair(T1 first, T2 second)
            : base(first, second) { }

        public int CompareTo(ComparablePair<T1, T2>? other)
        {
            if (other is null) return -1;

            int result = First.CompareTo(other.First);

            if (result == 0)
            {
                result = Second.CompareTo(other.Second);
            }

            return result;
        }

        public static bool operator >(ComparablePair<T1, T2> lhs, ComparablePair<T1, T2> rhs)
            => lhs.CompareTo(rhs) > 0;

        public static bool operator <(ComparablePair<T1, T2> lhs, ComparablePair<T1, T2> rhs)
            => lhs.CompareTo(rhs) < 0;

        public static bool operator >=(ComparablePair<T1, T2> lhs, ComparablePair<T1, T2> rhs)
            => lhs.CompareTo(rhs) >= 0;

        public static bool operator <=(ComparablePair<T1, T2> lhs, ComparablePair<T1, T2> rhs)
            => lhs.CompareTo(rhs) <= 0;
    }
}
