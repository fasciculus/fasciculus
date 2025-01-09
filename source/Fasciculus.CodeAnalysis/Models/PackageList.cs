using Fasciculus.Collections;
using Fasciculus.Net;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Models
{
    public class PackageList : SymbolDictionary<PackageSymbol>
    {
        public PackageList(IEnumerable<PackageSymbol> packages)
            : base(packages) { }

        public PackageSymbol Combine(string packageName)
        {
            SymbolName name = new SymbolName(packageName);
            UriPath link = new(name);

            if (Count == 0)
            {
                return new(name, link, [], []);
            }

            PackageSymbol result = new(name, link, [], []);

            this.Select(p => p.Clone()).Apply(result.MergeWith);
            result.ReBase(link);

            return result;
        }
    }
}
