using Fasciculus.CodeAnalysis.Frameworking;

namespace Fasciculus.CodeAnalysis.Models
{
    public interface IEnumSymbol : ITypeSymbol
    {

    }

    internal class EnumSymbol : TypeSymbol<EnumSymbol>, IEnumSymbol
    {
        public EnumSymbol(TargetFramework framework, string package, SymbolComment comment)
            : base(SymbolKind.Enum, framework, package, comment) { }

        private EnumSymbol(EnumSymbol other, bool clone)
            : base(other, clone) { }

        public EnumSymbol Clone()
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
