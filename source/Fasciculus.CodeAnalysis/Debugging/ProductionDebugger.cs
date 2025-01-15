using Fasciculus.Threading.Synchronization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Debugging
{
    public class ProductionDebugger : INodeDebugger
    {
        private readonly TaskSafeMutex mutex = new();
        private readonly SortedSet<ProductionDebuggerEntry> entries = [];

        private readonly INodeDebugger? nextDebugger;

        public List<ProductionDebuggerEntry> this[SyntaxKind left] => GetEntries(left);

        public ProductionDebugger(INodeDebugger? nextDebugger = null)
        {
            this.nextDebugger = nextDebugger;
        }

        public void Add(SyntaxNode node)
        {
            using Locker locker = Locker.Lock(mutex);

            entries.Add(new(node));
            nextDebugger?.Add(node);
        }

        public List<ProductionDebuggerEntry> GetEntries(SyntaxKind left)
        {
            using Locker locker = Locker.Lock(mutex);

            return new(entries.Where(p => p.Left == left));
        }
    }
}
