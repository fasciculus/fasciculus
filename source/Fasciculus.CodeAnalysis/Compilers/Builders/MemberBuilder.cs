using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public class MemberBuilder : TypedSymbolBuilder<MemberSymbol>
    {
        public MemberBuilder(CommentContext commentContext)
            : base(commentContext) { }

        public override MemberSymbol Build()
        {
            MemberSymbol member = new(Framework, Package, Comment)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Type = Type,
            };

            return member;
        }
    }
}
