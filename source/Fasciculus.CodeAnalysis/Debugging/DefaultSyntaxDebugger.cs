using Fasciculus.Threading.Synchronization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Debugging
{
    public class DefaultSyntaxDebugger : INodeDebugger
    {
        private readonly TaskSafeMutex mutex = new();

        private readonly Dictionary<SyntaxKind, SyntaxDebuggerEntry> entries;
        private readonly INodeDebugger? nextDebugger;

        public DefaultSyntaxDebugger(Dictionary<SyntaxKind, SyntaxDebuggerEntry> entries, INodeDebugger? nextDebugger = null)
        {
            this.entries = entries;
            this.nextDebugger = nextDebugger;
        }

        public DefaultSyntaxDebugger()
            : this(new SyntaxDebuggerKnownEntries()) { }

        public void Add(SyntaxNode node)
        {
            using Locker locker = Locker.Lock(mutex);

            SyntaxKind kind = node.Kind();
            IEnumerable<SyntaxKind> used = node.ChildNodes().Select(c => c.Kind());

            if (!entries.TryGetValue(kind, out SyntaxDebuggerEntry? entry))
            {
                entry = new([]);
                entries.Add(kind, entry);
            }

            entry.AddUsed(used);
            nextDebugger?.Add(node);
        }

        public Dictionary<SyntaxKind, SortedSet<SyntaxKind>> GetUnhandled()
        {
            using Locker locker = Locker.Lock(mutex);

            Dictionary<SyntaxKind, SortedSet<SyntaxKind>> result = [];

            foreach (var kvp in entries)
            {
                SortedSet<SyntaxKind> unhandled = kvp.Value.GetUnhandled();

                if (unhandled.Count > 0)
                {
                    result.Add(kvp.Key, unhandled);
                }
            }

            return result;
        }
    }
}
