using Fasciculus.CodeAnalysis.Frameworks;
using System.Collections.Generic;
using System.Diagnostics;

namespace Fasciculus.CodeAnalysis.Models
{
    [DebuggerDisplay("{Name}")]
    public class Symbol<T>
        where T : notnull, Symbol<T>
    {
        public SymbolName Name { get; }

        private readonly TargetFrameworks frameworks = [];

        public IEnumerable<TargetFramework> Frameworks => frameworks;

        public Symbol(SymbolName name, TargetFramework framework)
        {
            Name = name;

            frameworks.Add(framework);
        }

        public virtual void MergeWith(T other)
        {
            frameworks.Add(other.Frameworks);
        }
    }
}
