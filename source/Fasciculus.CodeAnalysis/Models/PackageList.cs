using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.Collections;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Fasciculus.CodeAnalysis.Models
{
    internal class PackageList : SymbolDictionary<PackageSymbol>
    {
        private static readonly string CombinedComment
            = "<comment><summary>" +
            "This is the combination of all currently documented packages. This package doesn't exist." +
            "</summary></comment>";

        public PackageList(IEnumerable<PackageSymbol> packages)
            : base(packages.Where(p => !p.IsEmpty)) { }

        public PackageSymbol Combine(string packageName, UriPath packageLink, CommentContext commentContext)
        {
            SymbolName name = new(packageName);
            SymbolComment comment = new(commentContext, XDocument.Parse(CombinedComment));
            UriPath link = new(name);

            PackageSymbol result = new(name, TargetFramework.UnsupportedFramework, comment, [])
            {
                Name = name,
                Link = link,
                Modifiers = PackageSymbol.PackageModifiers,
                RepositoryDirectory = packageLink,
            };

            this.Select(p => p.Clone()).Apply(result.MergeWith);
            result.ReBase(link);

            return result;
        }
    }
}
