using Fasciculus.Collections;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Fasciculus.CodeAnalysis.Models
{
    public class PackageList : SymbolDictionary<PackageSymbol>
    {
        private static readonly string CombinedComment
            = "<comment><summary>" +
            "This is the combination of all currently documented packages. This package doesn't exist." +
            "</summary></comment>";

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
            result.Comment = new(XDocument.Parse(CombinedComment));

            return result;
        }
    }
}
