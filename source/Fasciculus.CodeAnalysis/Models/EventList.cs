using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Models
{
    internal class EventList : SymbolDictionary<EventSymbol>
    {
        public EventList(IEnumerable<EventSymbol> events)
            : base(events) { }

        public EventList()
            : this([]) { }

        private EventList(EventList other, bool _)
            : base(other.Select(e => e.Clone())) { }

        public EventList Clone()
            => new(this, true);
    }
}
