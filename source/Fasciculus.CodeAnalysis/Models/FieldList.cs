using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Models
{
    internal class FieldList : SymbolDictionary<FieldSymbol>
    {
        public FieldList(IEnumerable<FieldSymbol> fields)
            : base(fields) { }

        public FieldList()
            : this([]) { }

        private FieldList(FieldList other, bool _)
            : base(other.Select(f => f.Clone())) { }

        public FieldList Clone()
            => new(this, true);
    }
}
