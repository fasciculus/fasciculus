using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    public interface IClassOrInterfaceSymbol : ISourceSymbol
    {
        public IEnumerable<IEventSymbol> Events { get; }

        public IEnumerable<IPropertySymbol> Properties { get; }

        public IEnumerable<IMethodSymbol> Methods { get; }
    }

    internal class ClassOrInterfaceSymbol<T> : SourceSymbol<T>, IClassOrInterfaceSymbol
        where T : notnull, ClassOrInterfaceSymbol<T>
    {
        private readonly EventList events;

        public IEnumerable<IEventSymbol> Events => events;

        private readonly PropertyList properties;

        public IEnumerable<IPropertySymbol> Properties => properties;

        private readonly MethodList methods;

        public IEnumerable<IMethodSymbol> Methods => methods;

        public ClassOrInterfaceSymbol(SymbolKind kind, TargetFramework framework, string package, SymbolComment comment)
            : base(kind, framework, package, comment)
        {
            events = [];
            properties = [];
            methods = [];
        }

        protected ClassOrInterfaceSymbol(T other, bool clone)
            : base(other, clone)
        {
            events = other.events.Clone();
            properties = other.properties.Clone();
            methods = other.methods.Clone();
        }

        public override void MergeWith(T other)
        {
            base.MergeWith(other);

            events.AddOrMergeWith(other.events);
            properties.AddOrMergeWith(other.properties);
            methods.AddOrMergeWith(other.methods);
        }

        public override void ReBase(UriPath newBase)
        {
            base.ReBase(newBase);

            events.ReBase(newBase);
            properties.ReBase(newBase);
            methods.ReBase(newBase);
        }

        public void Add(EventSymbol @event)
            => events.AddOrMergeWith(@event);

        public void Add(PropertySymbol property)
            => properties.AddOrMergeWith(property);

        public void Add(MethodSymbol method)
            => methods.AddOrMergeWith(method);
    }
}
