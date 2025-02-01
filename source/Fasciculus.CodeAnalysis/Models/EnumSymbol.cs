using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    public interface IEnumSymbol : ISymbol
    {
        public IEnumerable<IMemberSymbol> Members { get; }
    }

    internal class EnumSymbol : Symbol<EnumSymbol>, IEnumSymbol
    {
        private readonly MemberList members;

        public IEnumerable<IMemberSymbol> Members => members;

        public EnumSymbol(TargetFramework framework, string package, SymbolComment comment)
            : base(SymbolKind.Enum, framework, package, comment)
        {
            members = [];
        }

        private EnumSymbol(EnumSymbol other, bool clone)
            : base(other, clone)
        {
            members = other.members.Clone();
        }

        public EnumSymbol Clone()
        {
            return new(this, true)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
            };
        }

        public override void MergeWith(EnumSymbol other)
        {
            base.MergeWith(other);

            members.AddOrMergeWith(other.members);
        }

        public override void ReBase(UriPath newBase)
        {
            base.ReBase(newBase);

            members.ReBase(newBase);
        }

        public void Add(MemberSymbol member)
            => members.AddOrMergeWith(member);
    }
}
