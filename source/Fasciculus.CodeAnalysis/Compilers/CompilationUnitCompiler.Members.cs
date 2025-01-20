using Fasciculus.CodeAnalysis.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public partial class CompilationUnitCompiler
    {
        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            // ConstructorDeclaration
            // : ParameterList (BaseConstructorInitializer | ThisConstructorInitializer)? Block

            nodeDebugger.Add(node);

            string name = GetName(node.Identifier, null);
            SymbolModifiers modifiers = modifiersCompiler.Compile(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                PushComment();

                base.VisitConstructorDeclaration(node);

                PopComment();
            }
        }

        public override void VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node)
        {
            // HasTrivia: True
            // ConversionOperatorDeclaration
            // : IdentifierName ParameterList ArrowExpressionClause
            // 
            // may have Block?

            nodeDebugger.Add(node);

            string name = node.Type.ToString();
            SymbolModifiers modifiers = modifiersCompiler.Compile(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                PushComment();

                base.VisitConversionOperatorDeclaration(node);

                PopComment();
            }
        }

        public override void VisitDestructorDeclaration(DestructorDeclarationSyntax node)
        {
            // HasTrivia: True
            // DestructorDeclaration: ParameterList Block

            nodeDebugger.Add(node);

            SymbolModifiers modifiers = modifiersCompiler.Compile(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                PushComment();

                base.VisitDestructorDeclaration(node);

                PopComment();
            }
        }

        public override void VisitEventFieldDeclaration(EventFieldDeclarationSyntax node)
        {
            // HasTrivia: True
            // EventFieldDeclaration: VariableDeclaration

            nodeDebugger.Add(node);

            SymbolModifiers modifiers = modifiersCompiler.Compile(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                PushComment();

                base.VisitEventFieldDeclaration(node);

                PopComment();
            }
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            // HasTrivia: True
            // FieldDeclaration: VariableDeclaration

            nodeDebugger.Add(node);

            SymbolModifiers modifiers = modifiersCompiler.Compile(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                PushComment();

                base.VisitFieldDeclaration(node);

                PopComment();
            }
        }

        public override void VisitIndexerDeclaration(IndexerDeclarationSyntax node)
        {
            // HasTrivia: True
            // IndexerDeclaration
            // : PredefinedType BracketedParameterList ArrowExpressionClause

            nodeDebugger.Add(node);

            SymbolModifiers modifiers = modifiersCompiler.Compile(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                PushComment();

                base.VisitIndexerDeclaration(node);

                PopComment();
            }
        }

        public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            // covers SimpleMemberAccessExpression and PointerMemberAccessExpression
            // SimpleMemberAccessExpression: (IdentifierName | GenericName) IdentifierName

            nodeDebugger.Add(node);

            base.VisitMemberAccessExpression(node);
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

            SymbolModifiers modifiers = modifiersCompiler.Compile(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                PushComment();

                base.VisitMethodDeclaration(node);

                PopComment();
            }
        }

        public override void VisitOperatorDeclaration(OperatorDeclarationSyntax node)
        {
            // HasTrivia: True
            // OperatorDeclaration
            // : AttributeList IdentifierName ParameterList ArrowExpressionClause

            nodeDebugger.Add(node);

            SymbolModifiers modifiers = modifiersCompiler.Compile(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                PushComment();

                base.VisitOperatorDeclaration(node);

                PopComment();
            }
        }
    }
}