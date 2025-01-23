using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public class EnumMemberBuilder : MemberBuilder<MemberSymbol>
    {
        public override MemberSymbol Build()
        {
            MemberSymbol member = new(Framework, Package)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Comment = Comment,
                Type = Type,
            };

            return member;
        }
    }
}
