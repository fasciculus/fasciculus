using Fasciculus.CodeAnalysis.Frameworks;
using Fasciculus.Net;

namespace Fasciculus.CodeAnalysis.Models
{
    public class ClassSymbol : Symbol<ClassSymbol>
    {
        public ClassSymbol(SymbolName name, UriPath link, TargetFramework framework)
            : base(SymbolKind.Class, name, link, framework) { }

        private ClassSymbol(ClassSymbol other, bool clone)
            : base(other, clone) { }

        public ClassSymbol Clone()
            => new(this, true);
    }
}
