using Fasciculus.Threading.Synchronization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Debugging
{
    public class DefaultSyntaxDebugger : INodeDebugger
    {
        public INodeDebugger Next { get; }

        private readonly TaskSafeMutex mutex = new();
        private readonly Dictionary<SyntaxKind, SyntaxDebuggerEntry> entries;

        public DefaultSyntaxDebugger(Dictionary<SyntaxKind, SyntaxDebuggerEntry> entries, INodeDebugger? next = null)
        {
            Next = next ?? new NullNodeDebugger();

            this.entries = entries;
        }

        public DefaultSyntaxDebugger(INodeDebugger? next = null)
            : this(new SyntaxDebuggerKnownEntries(), next) { }

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
            Next.Add(node);
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
