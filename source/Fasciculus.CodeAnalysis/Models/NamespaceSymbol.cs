using Fasciculus.CodeAnalysis.Frameworks;
using Fasciculus.Net;

namespace Fasciculus.CodeAnalysis.Models
{
    public class NamespaceSymbol : Symbol<NamespaceSymbol>
    {
        public NamespaceSymbol(SymbolName name, UriPath link, TargetFramework framework)
            : base(name, link, framework) { }
    }
}
