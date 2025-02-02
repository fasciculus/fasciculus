using Fasciculus.CodeAnalysis.Frameworking;

namespace Fasciculus.CodeAnalysis.Models
{
    public interface IMethodSymbol : IInvokableSymbol
    {

    }

    internal class MethodSymbol : InvokableSymbol<MethodSymbol>, IMethodSymbol
    {
        public MethodSymbol(TargetFramework framework, string package, SymbolComment comment)
            : base(SymbolKind.Method, framework, package, comment) { }

        private MethodSymbol(MethodSymbol other, bool clone)
            : base(other, clone) { }

        public MethodSymbol Clone()
        {
            return new(this, true)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Type = Type,
                BareName = BareName,
            };
        }
    }
}
