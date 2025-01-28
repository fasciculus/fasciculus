using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    internal class AccessorBuilder : SymbolBuilder<AccessorSymbol>
    {
        public override AccessorSymbol Build(SymbolComment comment)
        {
            AccessorSymbol accessor = new(Framework, Package, comment)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
            };

            return accessor;
        }
    }
}
