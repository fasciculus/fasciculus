using Fasciculus.Threading.Synchronization;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Debugging
{
    public class DefaultAccessorDebugger : IAccessorDebugger
    {
        private readonly TaskSafeMutex mutex = new();

        private readonly SortedSet<SyntaxKind> handled;
        private readonly SortedSet<SyntaxKind> used = [];

        public DefaultAccessorDebugger()
        {
            handled = new([SyntaxKind.GetAccessorDeclaration, SyntaxKind.SetAccessorDeclaration]);
        }

        public void Add(AccessorDeclarationSyntax node)
        {
            using Locker locker = Locker.Lock(mutex);

            used.Add(node.Kind());
        }

        public SortedSet<SyntaxKind> GetUnhandled()
        {
            using Locker locker = Locker.Lock(mutex);

            SortedSet<SyntaxKind> unhandled = new(used);

            unhandled.ExceptWith(handled);

            return unhandled;
        }
    }
}
