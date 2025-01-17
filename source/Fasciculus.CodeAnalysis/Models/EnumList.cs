using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Models
{
    public class EnumList : SymbolDictionary<EnumSymbol>
    {
        public EnumList(IEnumerable<EnumSymbol> enums)
            : base(enums) { }

        public EnumList()
            : this([]) { }

        private EnumList(EnumList other, bool clone)
            : base(other.Select(e => e.Clone())) { }

        public EnumList Clone()
            => new(this, true);
    }
}
