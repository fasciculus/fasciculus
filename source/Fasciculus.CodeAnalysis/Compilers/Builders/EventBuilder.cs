using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    internal class EventBuilder : TypedSymbolBuilder<EventSymbol>
    {
        public override EventSymbol Build(SymbolComment comment)
        {
            EventSymbol @event = new(Framework, Package, comment)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Type = Type,
            };

            @event.AddSource(Source);

            return @event;
        }
    }
}
