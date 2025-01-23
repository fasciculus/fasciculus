using Fasciculus.Support;
using Microsoft.CodeAnalysis;

namespace Fasciculus.CodeAnalysis.Debugging
{
    public class NullNodeDebugger : INodeDebugger
    {
        public INodeDebugger Next => throw Ex.InvalidOperation();

        public void Add(SyntaxNode node) { }
    }
}
