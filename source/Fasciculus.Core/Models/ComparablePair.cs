using System;

namespace Fasciculus.Models
{
    public class ComparablePair<T1, T2> : EquatablePair<T1, T2>, IComparable<ComparablePair<T1, T2>>
        where T1 : IComparable<T1>, IEquatable<T1>
        where T2 : IComparable<T2>, IEquatable<T2>
    {
        public ComparablePair(T1 first, T2 second)
            : base(first, second) { }

        public int CompareTo(ComparablePair<T1, T2> other)
        {
            int result = First.CompareTo(other.First);

            if (result == 0)
            {
                result = Second.CompareTo(other.Second);
            }

            return result;
        }
    }
}
