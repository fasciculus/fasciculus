using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Indexing
{
    public class SymbolIndices
    {
        public required Dictionary<UriPath, Symbol> Symbols { get; init; }

        public required Dictionary<UriPath, PackageSymbol> Packages { get; init; }

        public required Dictionary<UriPath, NamespaceSymbol> Namespaces { get; init; }

        public required Dictionary<UriPath, ClassSymbol> Classes { get; init; }

        public static SymbolIndicesBuilder Create()
            => new();
    }
}
