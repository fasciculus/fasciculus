using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Fasciculus.CodeAnalysis.Debugging
{
    public interface IAccessorDebugger
    {
        public void Add(AccessorDeclarationSyntax node);
    }
}
