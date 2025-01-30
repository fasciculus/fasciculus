using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Models
{
    internal class ParameterList : SymbolList<ParameterSymbol>
    {
        public ParameterList(IEnumerable<ParameterSymbol> parameters)
            : base(parameters) { }

        public ParameterList()
            : this([]) { }

        private ParameterList(ParameterList other, bool _)
            : base(other.Select(f => f.Clone())) { }

        public ParameterList Clone()
            => new(this, true);
    }
}
