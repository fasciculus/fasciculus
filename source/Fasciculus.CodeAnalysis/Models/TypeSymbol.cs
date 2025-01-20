using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    public class TypeSymbol<T> : Symbol<T>
        where T : notnull, TypeSymbol<T>
    {
        private PropertyList properties;

        public IEnumerable<PropertySymbol> Properties => properties;

        private readonly SortedSet<UriPath> sources = [];

        public IEnumerable<UriPath> Sources => sources;

        public TypeSymbol(SymbolKind kind, TargetFramework framework, string package)
            : base(kind, framework, package)
        {
            properties = [];
        }

        protected TypeSymbol(T other, bool clone)
            : base(other, clone)
        {
            properties = other.properties.Clone();
            sources.UnionWith(other.sources);
        }

        public override void MergeWith(T other)
        {
            base.MergeWith(other);

            properties.AddOrMergeWith(other.properties);
            sources.UnionWith(other.sources);
        }

        public override void ReBase(UriPath newBase)
        {
            base.ReBase(newBase);

            properties.ReBase(newBase);
        }

        public void Add(PropertySymbol property)
            => properties.AddOrMergeWith(property);

        public void AddSource(UriPath source)
        {
            sources.Add(source);
        }
    }
}
