using Fasciculus.CodeAnalysis.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Fasciculus.CodeAnalysis.Compilers
{
    internal partial class CompilationUnitCompiler
    {
        public override void VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node)
        {
            // HasTrivia: True
            // ConversionOperatorDeclaration
            // : IdentifierName ParameterList ArrowExpressionClause
            // 
            // may have Block?

            NodeDebugger.Add(node);

            string name = node.Type.ToString();
            SymbolModifiers modifiers = GetModifiers(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                //PushComment();

                //base.VisitConversionOperatorDeclaration(node);

                //PopComment();
            }
        }

        public override void VisitDestructorDeclaration(DestructorDeclarationSyntax node)
        {
            // HasTrivia: True
            // DestructorDeclaration: ParameterList Block

            NodeDebugger.Add(node);

            SymbolModifiers modifiers = GetModifiers(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                //PushComment();

                //base.VisitDestructorDeclaration(node);

                //PopComment();
            }
        }

        public override void VisitIndexerDeclaration(IndexerDeclarationSyntax node)
        {
            // HasTrivia: True
            // IndexerDeclaration
            // : PredefinedType BracketedParameterList ArrowExpressionClause

            NodeDebugger.Add(node);

            SymbolModifiers modifiers = GetModifiers(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                //PushComment();

                //base.VisitIndexerDeclaration(node);

                //PopComment();
            }
        }

        public override void VisitOperatorDeclaration(OperatorDeclarationSyntax node)
        {
            // HasTrivia: True
            // OperatorDeclaration
            // : AttributeList IdentifierName ParameterList ArrowExpressionClause

            NodeDebugger.Add(node);

            SymbolModifiers modifiers = GetModifiers(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                //PushComment();

                //base.VisitOperatorDeclaration(node);

                //PopComment();
            }
        }
    }
}