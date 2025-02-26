using Fasciculus.CodeAnalysis.Frameworking;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Models
{
    public interface IPackageSymbol : ISymbol
    {
        public IEnumerable<INamespaceSymbol> Namespaces { get; }

        public Uri Repository { get; }
    }

    internal class PackageSymbol : Symbol<PackageSymbol>, IPackageSymbol
    {
        public static SymbolModifiers DefaultModifiers()
            => new() { IsPublic = true };

        private readonly NamespaceList namespaces;

        public IEnumerable<INamespaceSymbol> Namespaces => namespaces;

        public override bool IsAccessible => namespaces.HasAccessible;

        public required Uri Repository { get; init; }

        public PackageSymbol(SymbolName name, TargetFramework framework, SymbolComment comment,
            IEnumerable<CompilationUnitInfo> compilationUnits)
            : base(SymbolKind.Package, framework, name, comment)
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
                Link = Link,
                Modifiers = Modifiers,
                Repository = Repository,
            };
        }

        public override void MergeWith(PackageSymbol other)
        {
            base.MergeWith(other);

            namespaces.AddOrMergeWith(other.namespaces);
        }
    }
}
