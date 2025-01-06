using System.Diagnostics;

namespace Fasciculus.CodeAnalysis.Models
{
    [DebuggerDisplay("{Name}")]
    public class Symbol<T>
        where T : notnull, Symbol<T>
    {
        public string Name { get; }

        public Symbol(string name)
        {
            Name = name;
        }

        public virtual void MergeWith(Symbol<T> other)
        {
        }
    }
}
