using Microsoft.CodeAnalysis;

namespace Fasciculus.CodeAnalysis.Debugging
{
    public interface INodeDebugger
    {
        public void Add(SyntaxNode node);
    }
}
