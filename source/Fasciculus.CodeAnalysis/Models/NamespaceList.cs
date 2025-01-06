using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    public class NamespaceList : SymbolDictionary<NamespaceSymbol>
    {
        public NamespaceList(IEnumerable<NamespaceSymbol> namespaces)
            : base(namespaces) { }

        public NamespaceList()
            : this([]) { }
    }
}
