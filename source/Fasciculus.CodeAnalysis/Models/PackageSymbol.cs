using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    public class PackageSymbol : Symbol<PackageSymbol>
    {
        public PackageSymbol(string name, IEnumerable<CompilationUnit> compilationUnits)
            : base(name)
        {
        }
    }
}
