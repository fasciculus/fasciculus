using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    public class TypeSymbol<T> : SourceSymbol<T>
        where T : notnull, TypeSymbol<T>
    {
        private readonly FieldList fields;

        public IEnumerable<FieldSymbol> Fields => fields;

        private readonly PropertyList properties;

        public IEnumerable<PropertySymbol> Properties => properties;

        public TypeSymbol(SymbolKind kind, TargetFramework framework, string package)
            : base(kind, framework, package)
        {
            fields = [];
            properties = [];

        }

        protected TypeSymbol(T other, bool clone)
            : base(other, clone)
        {
            fields = other.fields.Clone();
            properties = other.properties.Clone();
        }

        public override void MergeWith(T other)
        {
            base.MergeWith(other);

            fields.AddOrMergeWith(other.fields);
            properties.AddOrMergeWith(other.properties);
        }

        public override void ReBase(UriPath newBase)
        {
            base.ReBase(newBase);

            fields.ReBase(newBase);
            properties.ReBase(newBase);
        }

        public void Add(FieldSymbol field)
            => fields.AddOrMergeWith(field);

        public void Add(PropertySymbol property)
            => properties.AddOrMergeWith(property);
    }
}
