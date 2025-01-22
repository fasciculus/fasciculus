using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public class EnumMemberBuilder : MemberBuilder<EnumMemberSymbol>
    {
        public override EnumMemberSymbol Build()
        {
            EnumMemberSymbol member = new(Framework, Package)
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
