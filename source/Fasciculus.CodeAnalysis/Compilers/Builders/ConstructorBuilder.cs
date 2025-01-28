using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    internal class ConstructorBuilder : InvokableBuilder<ConstructorSymbol>
    {
        public override ConstructorSymbol Build(SymbolComment comment)
        {
            ConstructorSymbol constructor = new(Framework, Package, comment)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Type = Type,
            };

            constructor.AddSource(Source);

            return constructor;
        }
    }
}
