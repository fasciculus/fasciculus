using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    internal class ParameterBuilder : TypedSymbolBuilder<ParameterSymbol>
    {
        public override ParameterSymbol Build(SymbolComment comment)
        {
            ParameterSymbol parameter = new(Framework, Package, comment)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Type = Type,
            };

            parameter.AddSource(Source);

            return parameter;
        }
    }
}
