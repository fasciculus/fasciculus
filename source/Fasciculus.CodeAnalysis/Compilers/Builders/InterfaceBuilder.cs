using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public class InterfaceBuilder : TypeBuilder
    {
        public InterfaceBuilder(SymbolName name, UriPath link, TargetFramework framework, string package, SymbolModifiers modifiers)
            : base(name, link, framework, package, modifiers)
        {
        }

        public InterfaceSymbol Build()
        {
            InterfaceSymbol @interface = new(Framework, Package)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Comment = Comment
            };

            return @interface;
        }
    }
}
