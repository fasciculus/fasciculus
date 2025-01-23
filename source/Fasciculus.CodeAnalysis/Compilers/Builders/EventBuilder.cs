using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public class EventBuilder : MemberBuilder<EventSymbol>
    {
        public EventBuilder(SymbolCommentContext commentContext)
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
