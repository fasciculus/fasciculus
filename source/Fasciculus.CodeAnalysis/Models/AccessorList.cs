using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Models
{
    internal class AccessorList : SymbolDictionary<AccessorSymbol>
    {
        public AccessorList(IEnumerable<AccessorSymbol> accessors)
            : base(accessors) { }

        public AccessorList()
            : this([]) { }

        private AccessorList(AccessorList other, bool _)
            : base(other.Select(a => a.Clone())) { }

        public AccessorList Clone()
            => new(this, true);

        public override string? ToString()
        {
            return Count == 0 ? string.Empty : $"{{ {string.Join(" ", this)} }}";
        }
    }
}
