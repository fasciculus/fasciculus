using Fasciculus.CodeAnalysis.Frameworking;

namespace Fasciculus.CodeAnalysis.Models
{
    public interface IEventSymbol : ITypedSymbol
    {

    }

    internal class EventSymbol : TypedSymbol<EventSymbol>, IEventSymbol
    {
        public EventSymbol(TargetFramework framework, string package, SymbolComment comment)
            : base(SymbolKind.Event, framework, package, comment) { }

        private EventSymbol(EventSymbol other, bool clone)
            : base(other, clone) { }

        public EventSymbol Clone()
        {
            return new(this, true)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Type = Type,
            };
        }

        protected override string GetId()
            => $"{Kind}-{Name.Name}";
    }
}
