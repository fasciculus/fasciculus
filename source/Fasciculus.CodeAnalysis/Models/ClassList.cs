using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    public class ClassList : SymbolDictionary<ClassSymbol>
    {
        public ClassList(IEnumerable<ClassSymbol> classes)
            : base(classes) { }

        public ClassList()
            : this([]) { }
    }
}
