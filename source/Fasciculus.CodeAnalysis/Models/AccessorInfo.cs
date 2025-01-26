using System;

namespace Fasciculus.CodeAnalysis.Models
{
    public enum AccessorKind
    {
        Unknown,
        Get,
        Set,
    }

    public static class AccessorKindExtensions
    {
        public static string AsString(this AccessorKind kind)
        {
            return kind switch
            {
                AccessorKind.Unknown => "unknown",
                AccessorKind.Get => "get",
                AccessorKind.Set => "set",
                _ => "unknown",
            };
        }
    }

    public interface IAccessorInfo : IEquatable<IAccessorInfo>, IComparable<IAccessorInfo>
    {
        public AccessorKind Kind { get; }

        public ISymbolModifiers Modifiers { get; }
    }

    internal class AccessorInfo : IAccessorInfo, IEquatable<AccessorInfo>, IComparable<AccessorInfo>
    {
        public required AccessorKind Kind { get; init; }

        public required ISymbolModifiers Modifiers { get; init; }

        public static AccessorInfo CreateUnknown(SymbolModifiers modifiers)
            => new() { Kind = AccessorKind.Unknown, Modifiers = modifiers };

        public static AccessorInfo CreateGet(SymbolModifiers modifiers)
            => new() { Kind = AccessorKind.Get, Modifiers = modifiers };

        public static AccessorInfo CreateSet(SymbolModifiers modifiers)
            => new() { Kind = AccessorKind.Set, Modifiers = modifiers };

        public bool Equals(IAccessorInfo? other)
            => other is not null && Kind == other.Kind;

        public bool Equals(AccessorInfo? other)
            => other is not null && Kind == other.Kind;

        public override bool Equals(object? obj)
            => Equals(obj as AccessorInfo);

        public override int GetHashCode()
            => Kind.GetHashCode();

        public int CompareTo(AccessorInfo? other)
            => other is null ? -1 : Kind.CompareTo(other.Kind);

        public int CompareTo(IAccessorInfo? other)
            => other is null ? -1 : Kind.CompareTo(other.Kind);

        public override string? ToString()
        {
            return $"{Modifiers} {Kind.AsString()};".TrimStart();
        }
    }
}
