using Fasciculus.CodeAnalysis.Frameworking;
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
            : base(packages.Where(p => !p.IsEmpty)) { }

        public PackageSymbol Combine(string packageName)
        {
            SymbolName name = new(packageName);
            UriPath link = new(name);
            PackageSymbol result = new(name, link, TargetFramework.UnsupportedFramework, []);

            this.Select(p => p.Clone()).Apply(result.MergeWith);
            result.ReBase(link);
            result.Comment = new(XDocument.Parse(CombinedComment));

            return result;
        }
    }
}
