using Fasciculus.CodeAnalysis.Frameworking;

namespace Fasciculus.CodeAnalysis.Models
{
    public class InterfaceSymbol : TypeSymbol<InterfaceSymbol>
    {
        public InterfaceSymbol(TargetFramework framework, string package, SymbolComment comment)
            : base(SymbolKind.Interface, framework, package, comment) { }

        private InterfaceSymbol(InterfaceSymbol other, bool clone)
            : base(other, clone) { }

        public InterfaceSymbol Clone()
        {
            return new(this, true)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
            };
        }
    }
}
