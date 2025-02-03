using Fasciculus.CodeAnalysis.Models;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    internal abstract class ClassOrInterfaceBuilder<T> : SymbolBuilder<T>, IEventReceiver, IPropertyReceiver, IMethodReceiver
        where T : notnull, ClassOrInterfaceSymbol<T>
    {
        private readonly EventList events = [];
        private readonly PropertyList properties = [];
        private readonly MethodList methods = [];

        protected IEnumerable<EventSymbol> Events => events;
        protected IEnumerable<PropertySymbol> Properties => properties;

        protected IEnumerable<MethodSymbol> Methods => methods;

        public void Add(EventSymbol @event)
            => events.AddOrMergeWith(@event);

        public void Add(PropertySymbol property)
            => properties.AddOrMergeWith(property);

        public void Add(MethodSymbol method)
            => methods.AddOrMergeWith(method);
    }
}
