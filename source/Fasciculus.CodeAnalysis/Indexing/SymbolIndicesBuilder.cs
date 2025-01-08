using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Indexing
{
    public class SymbolIndicesBuilder
    {
        private readonly List<PackageSymbol> packages = [];

        public SymbolIndicesBuilder WithPackages(IEnumerable<PackageSymbol> packages)
        {
            packages.Apply(this.packages.Add);

            return this;
        }

        public SymbolIndicesBuilder WithPackages(params PackageSymbol[] packages)
            => WithPackages(packages.AsEnumerable());

        public SymbolIndices Build()
        {
            SymbolIndicesFactory factory = new();

            return factory.CreateIndices(packages);
        }
    }
}
