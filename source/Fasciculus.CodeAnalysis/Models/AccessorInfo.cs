using System;
using System.Text;

namespace Fasciculus.CodeAnalysis.Models
{
    public enum AccessorKind
    {
        Get
    }

    public class AccessorInfo : IEquatable<AccessorInfo>, IComparable<AccessorInfo>
    {
        public required AccessorKind Kind { get; init; }

        public required SymbolModifiers Modifiers { get; init; }

        public static AccessorInfo Get(SymbolModifiers modifiers)
            => new() { Kind = AccessorKind.Get, Modifiers = modifiers };

        public bool Equals(AccessorInfo? other)
            => other is not null && Kind == other.Kind;

        public override bool Equals(object? obj)
            => Equals(obj as AccessorInfo);

        public override int GetHashCode()
            => Kind.GetHashCode();

        public int CompareTo(AccessorInfo? other)
            => other is null ? -1 : Kind.CompareTo(other.Kind);

        public override string? ToString()
        {
            StringBuilder sb = new();

            if (!Modifiers.IsPublic)
            {
                sb.Append(Modifiers.ToString()).Append(' ');
            }

            switch (Kind)
            {
                case AccessorKind.Get: sb.Append("get;"); break;
            }

            return sb.ToString();
        }
    }
}
