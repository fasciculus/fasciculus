using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    internal class PropertyBuilder : TypedSymbolBuilder<PropertySymbol>
    {
        public required AccessorList Accessors { get; init; }

        public override PropertySymbol Build(SymbolComment comment)
        {
            PropertySymbol property = new(Framework, Package, comment)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Type = Type,
            };

            Accessors.Apply(property.Add);

            property.AddSource(Source);

            return property;
        }
    }
}
