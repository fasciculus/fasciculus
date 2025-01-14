using Microsoft.CodeAnalysis;

namespace Fasciculus.CodeAnalysis.Debugging
{
    public interface INodeLogger
    {
        public void Add(SyntaxNode node);
    }
}
