using Fasciculus.CodeAnalysis.Frameworking;

namespace Fasciculus.CodeAnalysis.Models
{
    public class EventSymbol : MemberSymbol<EventSymbol>
    {
        public EventSymbol(TargetFramework framework, string package)
            : base(SymbolKind.Event, framework, package) { }

        private EventSymbol(EventSymbol other, bool clone)
            : base(other, clone) { }

        public EventSymbol Clone()
        {
            return new(this, true)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Comment = Comment,
                Type = Type,
            };
        }
    }
}
