using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public class InterfaceBuilder : TypeBuilder<InterfaceSymbol>
    {
        public InterfaceBuilder(SymbolCommentContext commentContext)
            : base(commentContext) { }

        public override InterfaceSymbol Build()
        {
            InterfaceSymbol @interface = new(Framework, Package)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Comment = Comment
            };

            Populate(@interface);

            return @interface;
        }
    }
}
