using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Fasciculus.CodeAnalysis.Compilers
{
    internal partial class CompilationUnitCompiler
    {
        public override void VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            // VariableDeclarator: EqualsValueClause?

            //Productions.Instance.Add(node);

            //base.VisitVariableDeclarator(node);
        }

        public override void VisitArrowExpressionClause(ArrowExpressionClauseSyntax node)
        {
            //base.VisitArrowExpressionClause(node);
        }

        public override void VisitAttribute(AttributeSyntax node)
        {
            // Attribute
            // : IdentifierName
            // | IdentifierName AttributeArgumentList
            // | QualifiedName AttributeArgumentList

            //base.VisitAttribute(node);
        }

        public override void VisitAttributeTargetSpecifier(AttributeTargetSpecifierSyntax node)
        {
            // Leaf
        }

        public override void VisitBlock(BlockSyntax node)
        {
            // base.VisitBlock(node);
        }

        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            //Productions.Instance.Add(node);

            //base.VisitIdentifierName(node);
        }

        public override void VisitOmittedArraySizeExpression(OmittedArraySizeExpressionSyntax node)
        {
            // Leaf

            //Productions.Instance.Add(node);

            //base.VisitOmittedArraySizeExpression(node);
        }

    }
}