using Fasciculus.CodeAnalysis.Frameworking;

namespace Fasciculus.CodeAnalysis.Models
{
    public class PropertySymbol : Symbol<PropertySymbol>
    {
        private readonly AccessorList accessors;

        public AccessorList Accessors => accessors.Clone();

        public PropertySymbol(TargetFramework framework, string package)
            : base(SymbolKind.Property, framework, package)
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
            };
        }

        public void Add(AccessorInfo accessor)
        {
            accessors.Add(accessor);
        }
    }
}
