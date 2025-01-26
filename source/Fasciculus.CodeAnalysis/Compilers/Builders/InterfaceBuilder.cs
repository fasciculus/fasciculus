using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    internal class InterfaceBuilder : TypeBuilder<InterfaceSymbol>
    {
        public InterfaceBuilder(CommentContext commentContext)
            : base(commentContext) { }

        public override InterfaceSymbol Build()
        {
            InterfaceSymbol @interface = new(Framework, Package, Comment)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
            };

            Populate(@interface);

            return @interface;
        }
    }
}
