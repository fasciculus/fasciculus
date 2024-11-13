using System;

namespace Fasciculus.Models
{
    public class Id<T> : IEquatable<Id<T>>, IComparable<Id<T>>
        where T : notnull, IEquatable<T>, IComparable<T>
    {
        public T Value { get; }

        public Id(T value)
        {
            Value = value;
        }

        public override bool Equals(object? obj)
            => obj is Id<T> id && Value.Equals(id.Value);

        public bool Equals(Id<T> other)
            => Value.Equals(other.Value);

        public int CompareTo(Id<T> other)
            => Value.CompareTo(other.Value);

        public override int GetHashCode()
            => Value.GetHashCode();

        public override string? ToString()
            => Value.ToString();

        public static bool operator ==(Id<T> a, Id<T> b)
            => a.Value.Equals(b.Value);

        public static bool operator !=(Id<T> a, Id<T> b)
            => !a.Value.Equals(b.Value);

        public static bool operator <(Id<T> a, Id<T> b)
            => a.Value.CompareTo(b.Value) < 0;

        public static bool operator >(Id<T> a, Id<T> b)
            => a.Value.CompareTo(b.Value) > 0;

        public static bool operator <=(Id<T> a, Id<T> b)
            => a.Value.CompareTo(b.Value) <= 0;

        public static bool operator >=(Id<T> a, Id<T> b)
            => a.Value.CompareTo(b.Value) >= 0;
    }
}
