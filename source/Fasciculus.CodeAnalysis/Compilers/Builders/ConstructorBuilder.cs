using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    internal class ConstructorBuilder : InvokableBuilder<ConstructorSymbol>
    {
        public override ConstructorSymbol Build(SymbolComment comment)
        {
            ConstructorSymbol constructor = new(Framework, Package, comment)
            {
                Name = CreateName(),
                Link = CreateLink(),
                Modifiers = Modifiers,
                Type = Type,
            };

            Parameters.Apply(constructor.Add);
            constructor.AddSource(Source);

            return constructor;
        }
    }
}
