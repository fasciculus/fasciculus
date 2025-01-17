using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.Net.Navigating;

namespace Fasciculus.CodeAnalysis.Models
{
    public class ClassSymbol : Symbol<ClassSymbol>
    {
        public ClassSymbol(SymbolName name, UriPath link, TargetFramework framework, string package)
            : base(SymbolKind.Class, name, link, framework, package) { }

        private ClassSymbol(ClassSymbol other, bool clone)
            : base(other, clone) { }

        public ClassSymbol Clone()
            => new(this, true);
    }
}
