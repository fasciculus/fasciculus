using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    internal class PropertyBuilder : TypedSymbolBuilder<PropertySymbol>, IAccessorReceiver
    {
        private readonly AccessorList accessors = [];

        public override PropertySymbol Build(SymbolComment comment)
        {
            PropertySymbol property = new(Framework, Package, comment)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Type = Type,
            };

            if (accessors.Count == 0)
            {
                SymbolComment accessorComment = SymbolComment.Empty(comment.Context);

                accessors.AddOrMergeWith(AccessorSymbol.CreateGet(Framework, Package, accessorComment, Link, Modifiers));
            }

            accessors.Apply(property.Add);

            property.AddSource(Source);

            return property;
        }

        public void Add(AccessorSymbol accessor)
            => accessors.AddOrMergeWith(accessor);
    }
}
