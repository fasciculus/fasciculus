using Fasciculus.CodeAnalysis.Frameworks;
using System.Collections.Generic;
using System.Diagnostics;

namespace Fasciculus.CodeAnalysis.Models
{
    public class Symbol
    {
        public SymbolName Name { get; }

        private readonly TargetFrameworks frameworks = [];

        public IEnumerable<TargetFramework> Frameworks => frameworks;

        public Symbol(SymbolName name, TargetFramework framework)
        {
            Name = name;

            frameworks.Add(framework);
        }

        public virtual void MergeWith(Symbol other)
        {
            frameworks.Add(other.Frameworks);
        }
    }

    [DebuggerDisplay("{Name}")]
    public class Symbol<T> : Symbol
        where T : notnull, Symbol<T>
    {
        public Symbol(SymbolName name, TargetFramework framework)
            : base(name, framework) { }

        public virtual void MergeWith(T other)
        {
            base.MergeWith(other);
        }
    }
}
