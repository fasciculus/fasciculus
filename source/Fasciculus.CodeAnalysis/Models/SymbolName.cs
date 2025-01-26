using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

        public SymbolName(string name)
        {
            Name = name;
            Mangled = name;
        }

        public SymbolName(string untyped, IEnumerable<string> parameterTypes)
        {
            bool hasParameters = parameterTypes.Any();

            Name = hasParameters ? $"{untyped}<{string.Join(',', parameterTypes)}>" : untyped;
            Mangled = hasParameters ? $"{untyped}-{parameterTypes.Count()}" : untyped;
        }

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
