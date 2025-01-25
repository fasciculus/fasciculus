using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Models
{
    public class ConstructorList : SymbolDictionary<ConstructorSymbol>
    {
        public ConstructorList(IEnumerable<ConstructorSymbol> constructors)
            : base(constructors) { }

        public ConstructorList()
            : this([]) { }

        private ConstructorList(ConstructorList other, bool _)
            : base(other.Select(c => c.Clone())) { }

        public ConstructorList Clone()
            => new(this, true);
    }
}
