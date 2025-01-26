using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Models
{
    internal class MemberList : SymbolDictionary<MemberSymbol>
    {
        public MemberList(IEnumerable<MemberSymbol> members)
            : base(members) { }

        public MemberList()
            : this([]) { }

        private MemberList(MemberList other, bool _)
            : base(other.Select(p => p.Clone())) { }

        public MemberList Clone()
            => new(this, true);
    }
}
