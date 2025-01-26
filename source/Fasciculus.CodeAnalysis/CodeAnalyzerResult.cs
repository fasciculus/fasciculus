using Fasciculus.CodeAnalysis.Indexing;
using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis
{
    public interface ICodeAnalyzerResult
    {
        public IPackageSymbol[] Packages { get; }
        public IPackageSymbol Combined { get; }
        public ISymbolIndex Index { get; }
    }

    internal class CodeAnalyzerResult : ICodeAnalyzerResult
    {
        private readonly PackageList packages;
        private readonly PackageSymbol combined;
        private readonly SymbolIndex index;

        public IPackageSymbol[] Packages => [.. packages];
        public IPackageSymbol Combined => combined;
        public ISymbolIndex Index => index;

        public CodeAnalyzerResult(PackageList packages, PackageSymbol combined, SymbolIndex index)
        {
            this.packages = packages;
            this.combined = combined;
            this.index = index;
        }
    }
}
