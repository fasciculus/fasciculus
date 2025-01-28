using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    internal class MemberBuilder : TypedSymbolBuilder<MemberSymbol>
    {
        public override MemberSymbol Build(SymbolComment comment)
        {
            MemberSymbol member = new(Framework, Package, comment)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Type = Type,
            };

            member.AddSource(Source);

            return member;
        }
    }
}
