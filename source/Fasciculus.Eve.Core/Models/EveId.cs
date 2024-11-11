using Fasciculus.IO;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Fasciculus.Eve.Models
{
    public readonly struct EveId : IEquatable<EveId>, IComparable<EveId>
    {
        public readonly int Value;

        public EveId(int value)
        {
            Value = value;
        }

        public static implicit operator EveId(int value)
            => new(value);

        public void Write(Stream stream)
        {
            Data data = stream;

            data.WriteInt(Value);
        }

        public static EveId Read(Stream stream)
        {
            Data data = stream;

            return data.ReadInt();
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
            => obj is EveId id && Value == id.Value;

        public bool Equals(EveId other)
            => Value == other.Value;

        public int CompareTo(EveId other)
            => Value.CompareTo(other.Value);

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string? ToString()
        {
            return Value.ToString();
        }

        public static bool operator ==(EveId a, EveId b)
            => a.Value == b.Value;

        public static bool operator !=(EveId a, EveId b)
            => a.Value == b.Value;

        public static bool operator <(EveId a, EveId b)
            => a.Value < b.Value;

        public static bool operator >(EveId a, EveId b)
            => a.Value > b.Value;

        public static bool operator <=(EveId a, EveId b)
            => a.Value <= b.Value;

        public static bool operator >=(EveId a, EveId b)
            => a.Value >= b.Value;
    }
}
