using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Models
{
    internal class MethodList : SymbolDictionary<MethodSymbol>
    {
        public MethodList(IEnumerable<MethodSymbol> methods)
            : base(methods) { }

        public MethodList()
            : this([]) { }

        private MethodList(MethodList other, bool _)
            : base(other.Select(m => m.Clone())) { }

        public MethodList Clone()
            => new(this, true);
    }
}
