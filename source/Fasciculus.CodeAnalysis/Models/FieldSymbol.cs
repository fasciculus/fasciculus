using Fasciculus.CodeAnalysis.Frameworking;

namespace Fasciculus.CodeAnalysis.Models
{
    public class FieldSymbol : TypedSymbol<FieldSymbol>
    {
        public FieldSymbol(TargetFramework framework, string package)
            : base(SymbolKind.Field, framework, package) { }

        private FieldSymbol(FieldSymbol other, bool clone)
            : base(other, clone) { }

        public FieldSymbol Clone()
        {
            return new(this, true)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Comment = Comment,
                Type = Type,
            };
        }
    }
}
