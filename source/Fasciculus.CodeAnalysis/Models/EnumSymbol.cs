using Fasciculus.CodeAnalysis.Frameworking;

namespace Fasciculus.CodeAnalysis.Models
{
    public class EnumSymbol : TypeSymbol<EnumSymbol>
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
