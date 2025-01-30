using System;
using System.Diagnostics;

namespace Fasciculus.CodeAnalysis.Models
{
    /// <summary>
    /// Immutable representation of a symbol's name.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public class SymbolName : IEquatable<SymbolName>, IComparable<SymbolName>
    {
        public static readonly StringComparer Comparer = StringComparer.Ordinal;

        public string Name { get; }

        public string Mangled { get; }

        public SymbolName(string name, string mangled)
        {
            Name = name;
            Mangled = mangled;
        }

        public SymbolName(string name)
            : this(name, name) { }

        public bool Equals(SymbolName? other)
            => Comparer.Equals(Name, other?.Name);

        public override bool Equals(object? obj)
            => Equals(obj as SymbolName);

        public override int GetHashCode()
            => Name.GetHashCode();

        public int CompareTo(SymbolName? other)
            => Comparer.Compare(Name, other?.Name);

        public override string? ToString()
            => Name;

        public static implicit operator string(SymbolName name) => name.Name;
    }
}
