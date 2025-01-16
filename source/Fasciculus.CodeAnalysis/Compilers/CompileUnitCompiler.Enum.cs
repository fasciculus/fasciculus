using Fasciculus.CodeAnalysis.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public partial class CompileUnitCompiler
    {
        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            // HasTrivia: True
            // EnumDeclaration
            // : EnumMemberDeclaration*

            nodeDebugger.Add(node);

            SymbolModifiers modifiers = modifierCompiler.Compile(node.Modifiers);

            PushComment();

            base.VisitEnumDeclaration(node);

            PopComment();
        }

        public override void VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
        {
            // HasTrivia: True
            // Leaf

            nodeDebugger.Add(node);

            SymbolModifiers modifiers = modifierCompiler.Compile(node.Modifiers);

            PushComment();

            base.VisitEnumMemberDeclaration(node);

            PopComment();
        }

    }
}