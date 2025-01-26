using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Indexing
{
    internal class SymbolIndexBuilder
    {
        private readonly SymbolIndexOptions options;

        private readonly List<PackageSymbol> packages = [];

        public SymbolIndexBuilder(SymbolIndexOptions options)
        {
            this.options = options;
        }

        public SymbolIndexBuilder WithPackages(IEnumerable<PackageSymbol> packages)
        {
            packages.Apply(this.packages.Add);

            return this;
        }

        public SymbolIndexBuilder WithPackages(params PackageSymbol[] packages)
            => WithPackages(packages.AsEnumerable());

        public SymbolIndex Build()
        {
            SymbolIndexFactory factory = new(options);

            return factory.Create(packages);
        }
    }
}
