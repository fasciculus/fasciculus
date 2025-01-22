using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Models
{
    public class EnumMemberList : SymbolDictionary<EnumMemberSymbol>
    {
        public EnumMemberList(IEnumerable<EnumMemberSymbol> members)
            : base(members) { }

        public EnumMemberList()
            : this([]) { }

        private EnumMemberList(EnumMemberList other, bool _)
            : base(other.Select(p => p.Clone())) { }

        public EnumMemberList Clone()
            => new(this, true);
    }
}
