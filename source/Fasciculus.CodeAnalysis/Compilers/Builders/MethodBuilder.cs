using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    internal class MethodBuilder : InvokableBuilder<MethodSymbol>
    {
        public override MethodSymbol Build(SymbolComment comment)
        {
            MethodSymbol method = new(Framework, Package, comment)
            {
                Name = CreateName(),
                Link = CreateLink(),
                Modifiers = Modifiers,
                Type = Type,
                BareName = BareName,
            };

            Parameters.Apply(method.Add);
            method.AddSource(Source);

            return method;
        }
    }
}
