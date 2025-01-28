using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    internal class EnumBuilder : SymbolBuilder<EnumSymbol>, IMemberReceiver
    {
        private readonly MemberList members = [];

        public override EnumSymbol Build(SymbolComment comment)
        {
            EnumSymbol @enum = new(Framework, Package, comment)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
            };

            members.Apply(@enum.Add);

            @enum.AddSource(Source);

            return @enum;
        }

        public void Add(MemberSymbol member)
            => members.AddOrMergeWith(member);
    }
}
