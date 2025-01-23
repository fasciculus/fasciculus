using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public class PropertyBuilder : TypedSymbolBuilder<PropertySymbol>
    {
        public required AccessorList Accessors { get; init; }

        public PropertyBuilder(SymbolCommentContext commentContext)
            : base(commentContext) { }

        public override PropertySymbol Build()
        {
            PropertySymbol property = new(Framework, Package)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Comment = Comment,
                Type = Type,
            };

            Accessors.Apply(property.Add);

            return property;
        }
    }
}
