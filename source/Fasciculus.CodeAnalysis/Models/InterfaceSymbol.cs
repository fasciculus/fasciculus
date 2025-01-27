using Fasciculus.CodeAnalysis.Frameworking;

namespace Fasciculus.CodeAnalysis.Models
{
    public interface IInterfaceSymbol : IClassOrInterfaceSymbol
    {

    }

    internal class InterfaceSymbol : ClassOrInterfaceSymbol<InterfaceSymbol>, IInterfaceSymbol
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
