using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public class ClassBuilder : TypeBuilder, ICommentReceiver
    {
        public ClassBuilder(SymbolName name, UriPath link, TargetFramework framework, string package, SymbolModifiers modifiers)
            : base(name, link, framework, package, modifiers)
        {
        }

        public ClassSymbol Build()
        {
            ClassSymbol @class = new(Name, Framework, Package)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Comment = Comment
            };

            return @class;
        }
    }
}
