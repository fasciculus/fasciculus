using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Models
{
    public class NamespaceList : SymbolDictionary<NamespaceSymbol>
    {
        public NamespaceList(IEnumerable<NamespaceSymbol> namespaces)
            : base(namespaces) { }

        public NamespaceList()
            : this([]) { }

        private NamespaceList(NamespaceList other, bool clone)
            : base(other.Select(n => n.Clone())) { }

        public NamespaceList Clone()
            => new(this, true);
    }
}
