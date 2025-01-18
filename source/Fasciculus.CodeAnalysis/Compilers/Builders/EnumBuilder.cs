using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public class EnumBuilder : TypeBuilder, ICommentReceiver
    {
        public EnumBuilder(SymbolName name, UriPath link, TargetFramework framework, string package, SymbolModifiers modifiers)
            : base(name, link, framework, package, modifiers)
        {
        }

        public EnumSymbol Build()
        {
            EnumSymbol @enum = new(Link, Framework, Package)
            {
                Name = Name,
                Modifiers = Modifiers,
                Comment = Comment
            };

            return @enum;
        }
    }
}
