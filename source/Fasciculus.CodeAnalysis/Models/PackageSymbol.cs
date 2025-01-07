using Fasciculus.CodeAnalysis.Frameworks;
using Fasciculus.Net;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Models
{
    public class PackageSymbol : Symbol<PackageSymbol>
    {
        private readonly NamespaceList namespaces;

        public PackageSymbol(SymbolName name, UriPath link, TargetFramework framework, IEnumerable<CompilationUnit> compilationUnits)
            : base(name, link, framework)
        {
            namespaces = new(compilationUnits.SelectMany(x => x.Namespaces));
        }

        public override void MergeWith(PackageSymbol other)
        {
            base.MergeWith(other);

            namespaces.AddOrMergeWith(other.namespaces);
        }
    }
}
