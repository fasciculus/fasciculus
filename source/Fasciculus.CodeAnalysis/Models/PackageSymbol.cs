using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Models
{
    public class PackageSymbol : Symbol<PackageSymbol>
    {
        private readonly NamespaceList namespaces;

        public IEnumerable<NamespaceSymbol> Namespaces => namespaces;

        public override bool IsAccessible => namespaces.HasAccessible;

        public PackageSymbol(SymbolName name, UriPath link, TargetFrameworks frameworks, IEnumerable<CompilationUnit> compilationUnits)
            : base(SymbolKind.Package, name, link, frameworks)
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
