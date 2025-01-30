using Fasciculus.CodeAnalysis.Frameworking;

namespace Fasciculus.CodeAnalysis.Models
{
    public interface IParameterSymbol : ITypedSymbol
    {
    }

    internal class ParameterSymbol : TypedSymbol<ParameterSymbol>, IParameterSymbol
    {
        public ParameterSymbol(TargetFramework framework, string package, SymbolComment comment)
            : base(SymbolKind.Parameter, framework, package, comment) { }

        private ParameterSymbol(ParameterSymbol other, bool clone)
            : base(other, clone) { }

        public ParameterSymbol Clone()
        {
            return new(this, true)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Type = Type,
            };
        }
    }
}
