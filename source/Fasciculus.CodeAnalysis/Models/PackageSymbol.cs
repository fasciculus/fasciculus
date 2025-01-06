using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    public class PackageSymbol : Symbol<PackageSymbol>
    {
        public PackageSymbol(SymbolName name, IEnumerable<CompilationUnit> compilationUnits)
            : base(name)
        {
        }
    }
}
