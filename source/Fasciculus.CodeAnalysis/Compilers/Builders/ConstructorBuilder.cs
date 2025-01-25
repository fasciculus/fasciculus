using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public class ConstructorBuilder : InvokableBuilder<ConstructorSymbol>
    {
        public ConstructorBuilder(CommentContext commentContext)
            : base(commentContext) { }

        public override ConstructorSymbol Build()
        {
            ConstructorSymbol constructor = new(Framework, Package)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Comment = Comment,
                Type = Type,
            };

            Populate(constructor);

            return constructor;
        }
    }
}
