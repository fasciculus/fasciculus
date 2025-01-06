using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    public class PackageList : SymbolDictionary<PackageSymbol>
    {
        public PackageList(IEnumerable<PackageSymbol> packages)
            : base(packages) { }
    }
}
