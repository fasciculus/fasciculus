using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Fasciculus.CodeAnalysis.Indexing
{
    public class SymbolIndex
    {
        private readonly Dictionary<UriPath, Symbol> index;

        public IEnumerable<UriPath> Links => index.Keys;

        public IEnumerable<Symbol> Symbols => index.Values;

        public Symbol this[UriPath link] => index[link];

        public SymbolIndex(Dictionary<UriPath, Symbol> index)
        {
            this.index = new(index);
        }

        public bool TryGetSymbol(UriPath link, [MaybeNullWhen(false)] out Symbol? symbol)
            => index.TryGetValue(link, out symbol);

        public static SymbolIndexBuilder Create(SymbolIndexOptions options)
            => new(options);
    }
}
