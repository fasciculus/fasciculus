using Fasciculus.CodeAnalysis.Indexing;
using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis
{
    public interface ICodeAnalyzerResult
    {
        public IPackageSymbol[] Packages { get; }
        public ISymbolIndex Index { get; }
    }

    internal class CodeAnalyzerResult : ICodeAnalyzerResult
    {
        private readonly PackageList packages;
        private readonly SymbolIndex index;

        public IPackageSymbol[] Packages => [.. packages];
        public ISymbolIndex Index => index;

        public CodeAnalyzerResult(PackageList packages, SymbolIndex index)
        {
            this.packages = packages;
            this.index = index;
        }
    }
}
