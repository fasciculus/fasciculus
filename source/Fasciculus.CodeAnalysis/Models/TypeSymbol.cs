using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    public interface ITypeSymbol : ISourceSymbol
    {
        public IEnumerable<IMemberSymbol> Members { get; }

        public IEnumerable<IFieldSymbol> Fields { get; }

        public IEnumerable<IEventSymbol> Events { get; }

        public IEnumerable<IPropertySymbol> Properties { get; }

        public IEnumerable<IMethodSymbol> Methods { get; }
    }

    internal class TypeSymbol<T> : SourceSymbol<T>, ITypeSymbol
        where T : notnull, TypeSymbol<T>
    {
        private readonly FieldList fields;

        public IEnumerable<IFieldSymbol> Fields => fields;

        private readonly MemberList members;

        public IEnumerable<IMemberSymbol> Members => members;

        private readonly EventList events;

        public IEnumerable<IEventSymbol> Events => events;

        private readonly PropertyList properties;

        public IEnumerable<IPropertySymbol> Properties => properties;

        private readonly MethodList methods;

        public IEnumerable<IMethodSymbol> Methods => methods;

        public TypeSymbol(SymbolKind kind, TargetFramework framework, string package, SymbolComment comment)
            : base(kind, framework, package, comment)
        {
            fields = [];
            members = [];
            events = [];
            properties = [];
            methods = [];
        }

        protected TypeSymbol(T other, bool clone)
            : base(other, clone)
        {
            fields = other.fields.Clone();
            members = other.members.Clone();
            events = other.events.Clone();
            properties = other.properties.Clone();
            methods = other.methods.Clone();
        }

        public override void MergeWith(T other)
        {
            base.MergeWith(other);

            fields.AddOrMergeWith(other.fields);
            members.AddOrMergeWith(other.members);
            events.AddOrMergeWith(other.events);
            properties.AddOrMergeWith(other.properties);
            methods.AddOrMergeWith(other.methods);
        }

        public override void ReBase(UriPath newBase)
        {
            base.ReBase(newBase);

            fields.ReBase(newBase);
            members.ReBase(newBase);
            events.ReBase(newBase);
            properties.ReBase(newBase);
            methods.ReBase(newBase);
        }

        public void Add(FieldSymbol field)
            => fields.AddOrMergeWith(field);

        public void Add(MemberSymbol member)
            => members.AddOrMergeWith(member);

        public void Add(EventSymbol @event)
            => events.AddOrMergeWith(@event);

        public void Add(PropertySymbol property)
            => properties.AddOrMergeWith(property);

        public void Add(MethodSymbol method)
            => methods.AddOrMergeWith(method);
    }
}
