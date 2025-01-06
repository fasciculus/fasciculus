using Fasciculus.CodeAnalysis.Frameworks;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    public class PackageSymbol : Symbol<PackageSymbol>
    {
        public PackageSymbol(SymbolName name, TargetFramework framework, IEnumerable<CompilationUnit> compilationUnits)
            : base(name, framework)
        {
        }
    }
}
