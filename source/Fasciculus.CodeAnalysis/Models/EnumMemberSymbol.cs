using Fasciculus.CodeAnalysis.Frameworking;

namespace Fasciculus.CodeAnalysis.Models
{
    public class EnumMemberSymbol : MemberSymbol<EnumMemberSymbol>
    {
        public EnumMemberSymbol(TargetFramework framework, string package)
            : base(SymbolKind.EnumMember, framework, package) { }

        private EnumMemberSymbol(EnumMemberSymbol other, bool clone)
            : base(other, clone) { }

        public EnumMemberSymbol Clone()
        {
            return new(this, true)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Comment = Comment,
                Type = Type,
            };
        }
    }
}
