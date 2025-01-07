using Fasciculus.CodeAnalysis.Frameworks;
using Fasciculus.Net;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Models
{
    public class PackageSymbol : Symbol<PackageSymbol>
    {
        private readonly NamespaceList namespaces;

        public IEnumerable<NamespaceSymbol> Namespaces => namespaces;

        public PackageSymbol(SymbolName name, UriPath link, TargetFramework framework, IEnumerable<CompilationUnit> compilationUnits)
            : base(name, link, framework)
        {
            namespaces = new(compilationUnits.SelectMany(x => x.Namespaces));
        }

        private PackageSymbol(PackageSymbol other, bool clone)
            : base(other, clone)
        {
            namespaces = other.namespaces.Clone();
        }

        public PackageSymbol Clone()
            => new(this, true);

        public override void MergeWith(PackageSymbol other)
        {
            base.MergeWith(other);

            namespaces.AddOrMergeWith(other.namespaces);
        }

        public override void ReBase(UriPath newBase)
        {
            base.ReBase(newBase);

            namespaces.ReBase(newBase);
        }
    }
}
