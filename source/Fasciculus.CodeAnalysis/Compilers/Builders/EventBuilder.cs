using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    internal class EventBuilder : TypedSymbolBuilder<EventSymbol>
    {
        public EventBuilder(CommentContext commentContext)
            : base(commentContext) { }

        public override EventSymbol Build()
        {
            EventSymbol @event = new(Framework, Package, Comment)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Type = Type,
            };

            return @event;
        }
    }
}
