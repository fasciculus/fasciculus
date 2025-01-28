using Fasciculus.CodeAnalysis.Frameworking;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    public interface IPropertySymbol : ITypedSymbol
    {
        public IEnumerable<IAccessorSymbol> Accessors { get; }
    }

    internal class PropertySymbol : TypedSymbol<PropertySymbol>, IPropertySymbol
    {
        private readonly AccessorList accessors;

        public IEnumerable<IAccessorSymbol> Accessors => accessors;

        public PropertySymbol(TargetFramework framework, string package, SymbolComment comment)
            : base(SymbolKind.Property, framework, package, comment)
        {
            accessors = [];
        }

        private PropertySymbol(PropertySymbol other, bool clone)
            : base(other, clone)
        {
            accessors = other.accessors.Clone();
        }

        public PropertySymbol Clone()
        {
            return new(this, true)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Type = Type,
            };
        }

        public void Add(AccessorSymbol accessor)
        {
            accessors.AddOrMergeWith(accessor);
        }
    }
}
