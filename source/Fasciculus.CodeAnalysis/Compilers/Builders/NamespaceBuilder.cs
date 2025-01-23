using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public class NamespaceBuilder : TypeReceiver<NamespaceSymbol>
    {
        public NamespaceBuilder(SymbolCommentContext commentContext)
            : base(commentContext) { }

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
