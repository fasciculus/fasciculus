using Fasciculus.Threading.Synchronization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Debugging
{
    public class DefaultProductionDebugger : INodeDebugger
    {
        public INodeDebugger Next { get; }

        private readonly TaskSafeMutex mutex = new();
        private readonly SortedSet<ProductionDebuggerEntry> entries = [];

        public List<ProductionDebuggerEntry> this[SyntaxKind left] => GetEntries(left);

        public DefaultProductionDebugger(INodeDebugger? next = null)
        {
            Next = next ?? new NullNodeDebugger();
        }

        public void Add(SyntaxNode node)
        {
            using Locker locker = Locker.Lock(mutex);

            entries.Add(new(node));
            Next.Add(node);
        }

        public List<ProductionDebuggerEntry> GetEntries(SyntaxKind left)
        {
            using Locker locker = Locker.Lock(mutex);

            return new(entries.Where(p => p.Left == left));
        }
    }
}
