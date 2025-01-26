using Fasciculus.CodeAnalysis.Frameworking;

namespace Fasciculus.CodeAnalysis.Models
{
    public class PropertySymbol : TypedSymbol<PropertySymbol>
    {
        private readonly AccessorList accessors;

        public AccessorList Accessors => accessors.Clone();

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

        public void Add(AccessorInfo accessor)
        {
            accessors.Add(accessor);
        }
    }
}
