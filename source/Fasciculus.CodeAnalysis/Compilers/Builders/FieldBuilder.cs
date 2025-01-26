using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    internal class FieldBuilder : TypedSymbolBuilder<FieldSymbol>
    {
        public FieldBuilder(CommentContext commentContext)
            : base(commentContext) { }

        public override FieldSymbol Build()
        {
            FieldSymbol field = new(Framework, Package, Comment)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Type = Type,
            };

            return field;
        }
    }
}
