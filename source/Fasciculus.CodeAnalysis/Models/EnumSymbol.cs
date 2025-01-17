using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.Net.Navigating;

namespace Fasciculus.CodeAnalysis.Models
{
    public class EnumSymbol : Symbol<EnumSymbol>
    {
        public EnumSymbol(SymbolName name, UriPath link, TargetFramework framework, string package)
            : base(SymbolKind.Enum, name, link, framework, package) { }

        private EnumSymbol(EnumSymbol other, bool clone)
            : base(other, clone) { }

        public EnumSymbol Clone()
            => new(this, true);
    }
}
