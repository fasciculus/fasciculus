using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.Net;

namespace Fasciculus.CodeAnalysis.Models
{
    public class ClassSymbol : Symbol<ClassSymbol>
    {
        public ClassSymbol(SymbolName name, UriPath link, TargetFrameworks frameworks)
            : base(SymbolKind.Class, name, link, frameworks) { }

        private ClassSymbol(ClassSymbol other, bool clone)
            : base(other, clone) { }

        public ClassSymbol Clone()
            => new(this, true);
    }
}
