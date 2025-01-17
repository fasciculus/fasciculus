using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Indexing
{
    public class SymbolIndices
    {
        public required Dictionary<UriPath, Symbol> Symbols { get; init; }

        public static SymbolIndicesBuilder Create(SymbolIndicesOptions options)
            => new(options);
    }
}
