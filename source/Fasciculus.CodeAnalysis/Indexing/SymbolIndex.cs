using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Fasciculus.CodeAnalysis.Indexing
{
    public interface ISymbolIndex
    {
        public IEnumerable<ISymbol> Symbols { get; }

        public bool TryGetSymbol(UriPath link, [MaybeNullWhen(false)] out ISymbol? symbol);
    }

    internal class SymbolIndex : ISymbolIndex
    {
        private readonly Dictionary<UriPath, ISymbol> index;

        public IEnumerable<UriPath> Links => index.Keys;

        public IEnumerable<ISymbol> Symbols => index.Values;

        public ISymbol this[UriPath link] => index[link];

        public SymbolIndex(Dictionary<UriPath, ISymbol> index)
        {
            this.index = new(index);
        }

        public bool TryGetSymbol(UriPath link, [MaybeNullWhen(false)] out ISymbol? symbol)
            => index.TryGetValue(link, out symbol);

        public static SymbolIndexBuilder Create(SymbolIndexOptions options)
            => new(options);
    }
}
