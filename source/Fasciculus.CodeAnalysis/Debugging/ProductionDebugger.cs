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
        private readonly SortedSet<ProductionDebuggerProduction> productions = [];

        private readonly INodeDebugger? nextDebugger;

        public ProductionDebugger(INodeDebugger? nextDebugger = null)
        {
            this.nextDebugger = nextDebugger;
        }

        public void Add(SyntaxNode node)
        {
            using Locker locker = Locker.Lock(mutex);

            productions.Add(new(node));
            nextDebugger?.Add(node);
        }

        public List<ProductionDebuggerProduction> GetProductions(SyntaxKind left)
        {
            using Locker locker = Locker.Lock(mutex);

            return new(productions.Where(p => p.Left == left));
        }
    }
}
