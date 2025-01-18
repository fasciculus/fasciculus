using Fasciculus.CodeAnalysis.Frameworking;

namespace Fasciculus.CodeAnalysis.Models
{
    public class ClassSymbol : TypeSymbol<ClassSymbol>
    {
        public ClassSymbol(SymbolName name, TargetFramework framework, string package)
            : base(SymbolKind.Class, framework, package) { }

        private ClassSymbol(ClassSymbol other, bool clone)
            : base(other, clone) { }

        public ClassSymbol Clone()
        {
            return new(this, true)
            {
                Name = Name,
                Link = Link,
            };
        }
    }
}
