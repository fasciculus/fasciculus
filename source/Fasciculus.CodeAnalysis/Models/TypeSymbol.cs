using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    public class TypeSymbol<T> : Symbol<T>
        where T : notnull, TypeSymbol<T>
    {
        private readonly SortedSet<UriPath> sources = [];

        public IEnumerable<UriPath> Sources => sources;

        public TypeSymbol(SymbolKind kind, SymbolName name, UriPath link, TargetFramework framework, string package)
            : base(kind, name, link, framework, package)
        {
        }

        protected TypeSymbol(T other, bool clone)
            : base(other, clone)
        {
            sources.UnionWith(other.sources);
        }

        public override void MergeWith(T other)
        {
            base.MergeWith(other);

            sources.UnionWith(other.sources);
        }

        public void AddSource(UriPath source)
        {
            sources.Add(source);
        }
    }
}
