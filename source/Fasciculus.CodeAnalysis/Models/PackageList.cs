using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Models
{
    internal class PackageList : SymbolDictionary<PackageSymbol>
    {
        public PackageList(IEnumerable<PackageSymbol> packages)
            : base(packages.Where(p => p.Namespaces.Any())) { }
    }
}
