using Fasciculus.CodeAnalysis.Frameworks;
using Fasciculus.Net;

namespace Fasciculus.CodeAnalysis.Models
{
    public class ClassSymbol : Symbol<ClassSymbol>
    {
        public ClassSymbol(SymbolName name, UriPath link, TargetFramework framework)
            : base(name, link, framework) { }
    }
}
