using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public class EventBuilder : MemberBuilder<EventSymbol>
    {
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
