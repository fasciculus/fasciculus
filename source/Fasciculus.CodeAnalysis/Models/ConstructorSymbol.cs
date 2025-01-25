using Fasciculus.CodeAnalysis.Frameworking;

namespace Fasciculus.CodeAnalysis.Models
{
    public class ConstructorSymbol : InvokableSymbol<ConstructorSymbol>
    {
        public ConstructorSymbol(TargetFramework framework, string package)
            : base(SymbolKind.Constructor, framework, package) { }

        private ConstructorSymbol(ConstructorSymbol other, bool clone)
            : base(other, clone) { }

        public ConstructorSymbol Clone()
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
