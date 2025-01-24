using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public class FieldBuilder : TypedSymbolBuilder<FieldSymbol>
    {
        public FieldBuilder(CommentContext commentContext)
            : base(commentContext) { }

        public override FieldSymbol Build()
        {
            FieldSymbol field = new(Framework, Package)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Comment = Comment,
                Type = Type,
            };

            return field;
        }
    }
}
