using Fasciculus.CodeAnalysis.Frameworks;
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

        public PackageSymbol Merge(string packageName)
        {
            SymbolName name = new SymbolName(packageName);
            UriPath link = new(name.Name);

            if (Count == 0)
            {
                return new(name, link, TargetFramework.UnsupportedFramework, []);
            }

            TargetFramework framework = this.First().Frameworks.First();
            PackageSymbol result = new(name, link, framework, []);

            this.Select(p => p.Clone()).Apply(result.MergeWith);
            result.ReBase(link);

            return result;
        }
    }
}
