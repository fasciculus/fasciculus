using Fasciculus.CodeAnalysis.Frameworks;

namespace Fasciculus.CodeAnalysis.Models
{
    public class NamespaceSymbol : Symbol<NamespaceSymbol>
    {
        public NamespaceSymbol(SymbolName name, TargetFramework framework)
            : base(name, framework) { }
    }
}
