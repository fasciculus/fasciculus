using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Fasciculus.CodeAnalysis.Debugging
{
    public class NullAccessorDebugger : IAccessorDebugger
    {
        public void Add(AccessorDeclarationSyntax node) { }
    }
}
