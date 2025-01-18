using Fasciculus.CodeAnalysis.Frameworking;

namespace Fasciculus.CodeAnalysis.Models
{
    public class EnumSymbol : TypeSymbol<EnumSymbol>
    {
        public EnumSymbol(TargetFramework framework, string package)
            : base(SymbolKind.Enum, framework, package) { }

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
