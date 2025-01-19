using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Models
{
    public class InterfaceList : SymbolDictionary<InterfaceSymbol>
    {
        public InterfaceList(IEnumerable<InterfaceSymbol> interfaces)
            : base(interfaces) { }

        public InterfaceList()
            : this([]) { }

        private InterfaceList(InterfaceList other, bool clone)
            : base(other.Select(e => e.Clone())) { }

        public InterfaceList Clone()
            => new(this, true);
    }
}
