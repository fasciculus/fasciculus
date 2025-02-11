using Fasciculus.Collections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Models
{
    public interface ISymbolList
    {
        public int Count { get; }
    }

    internal class SymbolList<T> : IEnumerable<T>, ISymbolList
        where T : notnull, Symbol<T>
    {
        private readonly List<T> symbols = [];

        public int Count => symbols.Count;

        public bool HasAccessible => symbols.Any(s => s.IsAccessible);

        protected SymbolList(IEnumerable<T> symbols)
        {
            Add(symbols);
        }

        public void Add(T symbol)
            => symbols.Add(symbol);

        public void Add(IEnumerable<T> symbols)
            => symbols.Apply(Add);

        public IEnumerator<T> GetEnumerator()
            => symbols.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => symbols.GetEnumerator();
    }
}
