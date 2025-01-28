using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Fasciculus.CodeAnalysis.Compilers
{
    internal partial class CompilationUnitCompiler
    {
        public override void VisitArrowExpressionClause(ArrowExpressionClauseSyntax node)
        {
        }

        public override void VisitAttributeList(AttributeListSyntax node)
        {
            // HasTrivia
            // AttributeList
            // : AttributeTargetSpecifier Attribute
            // | Attribute
        }

        public override void VisitBlock(BlockSyntax node)
        {
        }

        public override void VisitConstructorInitializer(ConstructorInitializerSyntax node)
        {
        }

        public override void VisitEqualsValueClause(EqualsValueClauseSyntax node)
        {
            // EqualsValueClause
            // : ImplicitObjectCreationExpression
            // | FalseLiteralExpression
            // | NullLiteralExpression
            // | DefaultLiteralExpression
            // | CollectionExpression
        }

        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
        }

        public override void VisitOmittedArraySizeExpression(OmittedArraySizeExpressionSyntax node)
        {
        }

        public override void VisitTypeParameterList(TypeParameterListSyntax node)
        {
            // TypeParameterList: TypeParameter+
        }

        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
            // UsingDirective
            // : IdentifierName
            // | QualifiedName
        }

        public override void VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            //VariableDeclarator: EqualsValueClause?
        }
    }
}