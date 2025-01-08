using Fasciculus.CodeAnalysis.Indexing;
using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis
{
    public class CodeAnalyzerResult
    {
        public required PackageList Packages { get; init; }
        public required PackageSymbol Combined { get; init; }
        public required SymbolIndices Indices { get; init; }
    }
}
