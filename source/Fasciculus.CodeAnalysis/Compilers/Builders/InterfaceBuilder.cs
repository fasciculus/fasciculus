using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public class InterfaceBuilder : TypeBuilder<InterfaceSymbol>
    {
        public override InterfaceSymbol Build()
        {
            InterfaceSymbol @interface = new(Framework, Package)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Comment = Comment
            };

            return @interface;
        }
    }
}
