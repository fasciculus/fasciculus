using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Models
{
    internal class ClassList : SymbolDictionary<ClassSymbol>
    {
        public ClassList(IEnumerable<ClassSymbol> classes)
            : base(classes) { }

        public ClassList()
            : this([]) { }

        private ClassList(ClassList other, bool clone)
            : base(other.Select(c => c.Clone())) { }

        public ClassList Clone()
            => new(this, true);
    }
}
