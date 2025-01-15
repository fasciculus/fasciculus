using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Debugging
{
    public class SyntaxDebuggerEntry
    {
        private readonly SortedSet<SyntaxKind> handled;
        private readonly SortedSet<SyntaxKind> used = [];

        public SyntaxDebuggerEntry(IEnumerable<SyntaxKind> handled)
        {
            this.handled = new(handled);
        }

        public void AddUsed(IEnumerable<SyntaxKind> kinds)
        {
            used.UnionWith(kinds);
        }

        public SortedSet<SyntaxKind> GetUnhandled()
        {
            SortedSet<SyntaxKind> unhandled = new(used);

            unhandled.ExceptWith(handled);

            return unhandled;
        }
    }
}
