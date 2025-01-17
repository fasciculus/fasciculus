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

            nodeDebugger.Add(node);

            SymbolModifiers modifiers = modifierCompiler.Compile(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                PushComment();

                base.VisitInterfaceDeclaration(node);

                PopComment();
            }
        }
    }
}