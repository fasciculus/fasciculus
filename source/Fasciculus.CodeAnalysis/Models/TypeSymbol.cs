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

        private readonly MemberList members;

        public IEnumerable<MemberSymbol> Members => members;

        private readonly EventList events;

        public IEnumerable<EventSymbol> Events => events;

        private readonly PropertyList properties;

        public IEnumerable<PropertySymbol> Properties => properties;

        public TypeSymbol(SymbolKind kind, TargetFramework framework, string package, SymbolComment comment)
            : base(kind, framework, package, comment)
        {
            fields = [];
            members = [];
            events = [];
            properties = [];
        }

        protected TypeSymbol(T other, bool clone)
            : base(other, clone)
        {
            fields = other.fields.Clone();
            members = other.members.Clone();
            events = other.events.Clone();
            properties = other.properties.Clone();
        }

        public override void MergeWith(T other)
        {
            base.MergeWith(other);

            fields.AddOrMergeWith(other.fields);
            members.AddOrMergeWith(other.members);
            events.AddOrMergeWith(other.events);
            properties.AddOrMergeWith(other.properties);
        }

        public override void ReBase(UriPath newBase)
        {
            base.ReBase(newBase);

            fields.ReBase(newBase);
            members.ReBase(newBase);
            events.ReBase(newBase);
            properties.ReBase(newBase);
        }

        public void Add(FieldSymbol field)
            => fields.AddOrMergeWith(field);

        public void Add(MemberSymbol member)
            => members.AddOrMergeWith(member);

        public void Add(EventSymbol @event)
            => events.AddOrMergeWith(@event);

        public void Add(PropertySymbol property)
            => properties.AddOrMergeWith(property);
    }
}
