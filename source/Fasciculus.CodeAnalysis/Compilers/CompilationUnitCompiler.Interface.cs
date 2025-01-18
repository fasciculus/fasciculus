using Fasciculus.CodeAnalysis.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public partial class CompilationUnitCompiler
    {
        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            // HasTrivia: True
            // InterfaceDeclaration
            // : TypeParameterList? BaseList? TypeParameterConstraintClause? MethodDeclaration*

            NodeDebugger.Add(node);

            SymbolModifiers modifiers = ModifiersCompiler.Compile(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                PushComment();

                base.VisitInterfaceDeclaration(node);

                PopComment();
            }
        }
    }
}