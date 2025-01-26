using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    internal class ConstructorBuilder : InvokableBuilder<ConstructorSymbol>
    {
        public ConstructorBuilder(CommentContext commentContext)
            : base(commentContext) { }

        public override ConstructorSymbol Build()
        {
            ConstructorSymbol constructor = new(Framework, Package, Comment)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Type = Type,
            };

            Populate(constructor);

            return constructor;
        }
    }
}
