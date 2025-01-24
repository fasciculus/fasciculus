using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public class EnumBuilder : TypeBuilder<EnumSymbol>
    {
        public EnumBuilder(CommentContext commentContext)
            : base(commentContext) { }

        public override EnumSymbol Build()
        {
            EnumSymbol @enum = new(Framework, Package)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Comment = Comment
            };

            Populate(@enum);

            return @enum;
        }
    }
}
