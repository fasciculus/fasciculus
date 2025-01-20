using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    public class MemberSymbol<T> : Symbol<T>
        where T : notnull, MemberSymbol<T>
    {
        private readonly SortedSet<UriPath> sources = [];

        public IEnumerable<UriPath> Sources => sources;

        public required string Type { get; init; }

        public MemberSymbol(SymbolKind kind, TargetFramework framework, string package)
            : base(kind, framework, package)
        {
        }

        protected MemberSymbol(T other, bool clone)
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
