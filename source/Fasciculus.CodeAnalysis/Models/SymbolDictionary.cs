using Fasciculus.Collections;
using Fasciculus.Net;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Models
{
    public class SymbolDictionary<T> : IEnumerable<T>
        where T : notnull, Symbol<T>
    {
        private readonly Dictionary<SymbolName, T> symbols = [];

        public int Count => symbols.Count;

        public bool HasAccessible => symbols.Values.Any(s => s.IsAccessible);

        public SymbolDictionary(IEnumerable<T> symbols)
        {
            AddOrMergeWith(symbols);
        }

        public void AddOrMergeWith(T symbol)
        {
            if (symbols.TryGetValue(symbol.Name, out T? existing))
            {
                existing.MergeWith(symbol);
            }
            else
            {
                symbols.Add(symbol.Name, symbol);
            }
        }

        public void AddOrMergeWith(IEnumerable<T> symbols)
            => symbols.Apply(AddOrMergeWith);

        public virtual void ReBase(UriPath newBase)
        {
            symbols.Values.Apply(s => { s.ReBase(newBase); });
        }

        public IEnumerator<T> GetEnumerator()
            => symbols.Values.OrderBy(x => x.Name).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => symbols.Values.OrderBy(x => x.Name).GetEnumerator();
    }
}
