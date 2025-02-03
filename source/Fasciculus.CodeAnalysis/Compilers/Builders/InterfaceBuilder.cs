using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    internal class InterfaceBuilder : ClassOrInterfaceBuilder<InterfaceSymbol>
    {
        public override InterfaceSymbol Build(SymbolComment comment)
        {
            InterfaceSymbol @interface = new(Framework, Package, comment)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
            };

            Events.Apply(@interface.Add);
            Properties.Apply(@interface.Add);

            Methods.Apply(@interface.Add);

            @interface.AddSource(Source);

            return @interface;
        }
    }
}
