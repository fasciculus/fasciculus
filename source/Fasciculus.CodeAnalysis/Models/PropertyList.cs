using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Models
{
    internal class PropertyList : SymbolDictionary<PropertySymbol>
    {
        public PropertyList(IEnumerable<PropertySymbol> properties)
            : base(properties) { }

        public PropertyList()
            : this([]) { }

        private PropertyList(PropertyList other, bool _)
            : base(other.Select(p => p.Clone())) { }

        public PropertyList Clone()
            => new(this, true);
    }
}
