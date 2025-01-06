using System.Diagnostics;

namespace Fasciculus.CodeAnalysis.Models
{
    [DebuggerDisplay("{Name}")]
    public class Symbol<T>
        where T : notnull, Symbol<T>
    {
        public SymbolName Name { get; }

        public Symbol(SymbolName name)
        {
            Name = name;
        }

        public virtual void MergeWith(Symbol<T> other)
        {
        }
    }
}
