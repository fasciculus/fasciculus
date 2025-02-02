using Fasciculus.CodeAnalysis.Frameworking;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Models
{
    public interface IConstructorSymbol : IInvokableSymbol
    {
    }

    internal class ConstructorSymbol : InvokableSymbol<ConstructorSymbol>, IConstructorSymbol
    {
        public ConstructorSymbol(TargetFramework framework, string package, SymbolComment comment)
            : base(SymbolKind.Constructor, framework, package, comment) { }

        private ConstructorSymbol(ConstructorSymbol other, bool clone)
            : base(other, clone) { }

        public ConstructorSymbol Clone()
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

        protected override string GetId()
            => $"{Kind}{string.Join("", Parameters.Select(p => $"-{p.Type.Mangled}"))}";
    }
}
