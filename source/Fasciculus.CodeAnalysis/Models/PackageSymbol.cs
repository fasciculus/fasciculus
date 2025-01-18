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

        public bool IsEmpty => namespaces.Count == 0;

        public required UriPath RepositoryDirectory { get; init; }

        public PackageSymbol(SymbolName name, UriPath link, TargetFramework framework, IEnumerable<CompilationUnitInfo> compilationUnits)
            : base(SymbolKind.Package, link, framework, name)
        {
            namespaces = new(compilationUnits.SelectMany(x => x.Namespaces));
        }

        private PackageSymbol(PackageSymbol other, bool clone)
            : base(other, clone)
        {
            namespaces = other.namespaces.Clone();
        }

        public PackageSymbol Clone()
        {
            return new(this, true)
            {
                Name = Name,
                RepositoryDirectory = RepositoryDirectory
            };
        }

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
