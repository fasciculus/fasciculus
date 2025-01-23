using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public class NamespaceBuilder : TypeReceiver<NamespaceSymbol>
    {
        public override NamespaceSymbol Build()
        {
            NamespaceSymbol @namespace = new(Framework, Package)
            {
                Name = Name,
                Link = Link,
                Modifiers = NamespaceSymbol.NamespaceModifiers,
                Comment = Comment,
            };

            enums.Apply(@namespace.AddOrMergeWith);
            interfaces.Apply(@namespace.AddOrMergeWith);
            classes.Apply(@namespace.AddOrMergeWith);

            return @namespace;
        }
    }
}
