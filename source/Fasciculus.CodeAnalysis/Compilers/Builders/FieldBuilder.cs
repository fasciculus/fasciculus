using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    internal class FieldBuilder : TypedSymbolBuilder<FieldSymbol>
    {
        public override FieldSymbol Build(SymbolComment comment)
        {
            FieldSymbol field = new(Framework, Package, comment)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Type = Type,
            };

            field.AddSource(Source);

            return field;
        }
    }
}
