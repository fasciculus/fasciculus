using Fasciculus.CodeAnalysis.Frameworking;

namespace Fasciculus.CodeAnalysis.Models
{
    public interface IMemberSymbol : ITypedSymbol
    {

    }

    internal class MemberSymbol : TypedSymbol<MemberSymbol>, IMemberSymbol
    {
        public MemberSymbol(TargetFramework framework, string package, SymbolComment comment)
            : base(SymbolKind.Member, framework, package, comment) { }

        private MemberSymbol(MemberSymbol other, bool clone)
            : base(other, clone) { }

        public MemberSymbol Clone()
        {
            return new(this, true)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Type = Type,
            };
        }

        protected override string GetId()
            => $"{Kind}-{Name.Name}";
    }
}
