using Fasciculus.CodeAnalysis.Frameworks;
using Fasciculus.Net;
using System.Collections.Generic;
using System.Diagnostics;

namespace Fasciculus.CodeAnalysis.Models
{
    [DebuggerDisplay("{Name}")]
    public class Symbol<T>
        where T : notnull, Symbol<T>
    {
        public SymbolName Name { get; }

        public UriPath Link { get; private set; }

        private readonly TargetFrameworks frameworks;

        public IEnumerable<TargetFramework> Frameworks => frameworks;

        public Symbol(SymbolName name, UriPath link, TargetFramework framework)
        {
            Name = name;
            Link = link;

            frameworks = new(framework);
        }

        protected Symbol(Symbol<T> other, bool _)
        {
            Name = other.Name;
            Link = other.Link;

            frameworks = new(other.Frameworks);
        }

        public virtual void MergeWith(T other)
        {
            frameworks.Add(other.Frameworks);
        }

        public virtual void ReBase(UriPath newBase)
        {
            Link = Link.Replace(0, 1, newBase);
        }
    }
}
