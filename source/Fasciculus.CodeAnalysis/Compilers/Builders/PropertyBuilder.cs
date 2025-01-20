using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public class PropertyBuilder : SymbolBuilder<PropertySymbol>
    {
        public required AccessorList Accessors { get; init; }

        public override PropertySymbol Build()
        {
            PropertySymbol property = new(Framework, Package)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Comment = Comment,
            };

            Accessors.Apply(property.Add);

            return property;
        }
    }
}
