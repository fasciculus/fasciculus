using Fasciculus.CodeAnalysis.Frameworks;
using Fasciculus.Net;
using System.Collections.Generic;
using System.Diagnostics;

namespace Fasciculus.CodeAnalysis.Models
{
    public class Symbol
    {
        public SymbolName Name { get; }

        public UriPath Link { get; }

        private readonly TargetFrameworks frameworks = [];

        public IEnumerable<TargetFramework> Frameworks => frameworks;

        public Symbol(SymbolName name, UriPath link, TargetFramework framework)
        {
            Name = name;
            Link = link;

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
        public Symbol(SymbolName name, UriPath link, TargetFramework framework)
            : base(name, link, framework) { }

        public virtual void MergeWith(T other)
        {
            base.MergeWith(other);
        }
    }
}
