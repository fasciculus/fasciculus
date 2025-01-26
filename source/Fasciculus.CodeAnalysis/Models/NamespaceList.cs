using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Models
{
    internal class NamespaceList : SymbolDictionary<NamespaceSymbol>
    {
        public NamespaceList(IEnumerable<NamespaceSymbol> namespaces)
            : base(namespaces.Where(n => !n.IsEmpty)) { }

        public NamespaceList()
            : this([]) { }

        private NamespaceList(NamespaceList other, bool clone)
            : base(other.Select(n => n.Clone())) { }

        public NamespaceList Clone()
            => new(this, true);
    }
}
