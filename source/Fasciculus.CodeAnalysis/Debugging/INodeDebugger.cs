using Microsoft.CodeAnalysis;

namespace Fasciculus.CodeAnalysis.Debugging
{
    public interface INodeDebugger
    {
        public INodeDebugger Next { get; }

        public void Add(SyntaxNode node);
    }
}
