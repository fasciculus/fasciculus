using Fasciculus.CodeAnalysis.Frameworking;

namespace Fasciculus.CodeAnalysis.Models
{
    public class MemberSymbol : TypedSymbol<MemberSymbol>
    {
        public MemberSymbol(TargetFramework framework, string package)
            : base(SymbolKind.Member, framework, package) { }

        private MemberSymbol(MemberSymbol other, bool clone)
            : base(other, clone) { }

        public MemberSymbol Clone()
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
