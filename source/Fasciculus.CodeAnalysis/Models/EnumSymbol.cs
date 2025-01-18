using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.Net.Navigating;

namespace Fasciculus.CodeAnalysis.Models
{
    public class EnumSymbol : TypeSymbol<EnumSymbol>
    {
        public EnumSymbol(UriPath link, TargetFramework framework, string package)
            : base(SymbolKind.Enum, link, framework, package) { }

        private EnumSymbol(EnumSymbol other, bool clone)
            : base(other, clone) { }

        public EnumSymbol Clone()
        {
            return new(this, true)
            {
                Name = Name,
            };
        }
    }
}
