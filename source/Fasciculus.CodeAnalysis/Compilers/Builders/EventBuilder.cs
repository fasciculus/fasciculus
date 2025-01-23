using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public class EventBuilder : TypedSymbolBuilder<EventSymbol>
    {
        public EventBuilder(CommentContext commentContext)
            : base(commentContext) { }

        public override EventSymbol Build()
        {
            EventSymbol @event = new(Framework, Package)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Comment = Comment,
                Type = Type,
            };

            return @event;
        }
    }
}
