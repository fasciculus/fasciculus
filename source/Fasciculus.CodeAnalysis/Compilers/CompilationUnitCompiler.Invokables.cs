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

            nodeDebugger.Add(node);

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

            nodeDebugger.Add(node);

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

            nodeDebugger.Add(node);

            SymbolModifiers modifiers = GetModifiers(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                //PushComment();

                //base.VisitIndexerDeclaration(node);

                //PopComment();
            }
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            // HasTrivia: True
            // MethodDeclaration
            // : AttributeList? <return-type> ExplicitInterfaceSpecifier? ParameterList TypeParameterConstraintClause? (ArrowExpressionClause | Block)
            //
            // return-type
            // : IdentifierName TypeParameterList?
            // | GenericName TypeParameterList?
            // | PredefinedType TypeParameterList?
            // | ArrayType
            // | NullableType

            nodeDebugger.Add(node);

            SymbolModifiers modifiers = GetModifiers(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                //PushComment();

                //base.VisitMethodDeclaration(node);

                //PopComment();
            }
        }

        public override void VisitOperatorDeclaration(OperatorDeclarationSyntax node)
        {
            // HasTrivia: True
            // OperatorDeclaration
            // : AttributeList IdentifierName ParameterList ArrowExpressionClause

            nodeDebugger.Add(node);

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