using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Support;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class MethodCompiler : FilteredCompiler
    {
        private static readonly SyntaxKind[] HandledSymbols =
        [
            SyntaxKind.AttributeList,
            SyntaxKind.Attribute,

            SyntaxKind.ParameterList,

            SyntaxKind.PredefinedType,
            SyntaxKind.ArrayType,
            SyntaxKind.NullableType,
            SyntaxKind.TypeParameterList,
            SyntaxKind.TypeParameterConstraintClause,
            SyntaxKind.ExplicitInterfaceSpecifier,

            SyntaxKind.SingleLineDocumentationCommentTrivia,

            SyntaxKind.IfDirectiveTrivia,
            SyntaxKind.EndIfDirectiveTrivia,

            SyntaxKind.IdentifierName,
            SyntaxKind.GenericName,

            SyntaxKind.Block,
            SyntaxKind.ArrowExpressionClause,
        ];

        private readonly CommentCompiler commentCompiler = new();

        private SymbolComment comment = SymbolComment.Empty;

        public MethodCompiler()
            : base(HandledSymbols, SyntaxWalkerDepth.StructuredTrivia)
        {
        }

        public void Compile(MethodDeclarationSyntax node)
        {
            comment = SymbolComment.Empty;

            DefaultVisit(node);
        }

        public override void VisitAttribute(AttributeSyntax node)
        {
        }

        public override void VisitParameterList(ParameterListSyntax node)
        {
            SymbolCounters.Instance.Increment("ParameterListSyntax");
        }

        public override void VisitPredefinedType(PredefinedTypeSyntax node)
        {
            SymbolCounters.Instance.Increment("PredefinedTypeSyntax");
        }

        public override void VisitArrayType(ArrayTypeSyntax node)
        {
            SymbolCounters.Instance.Increment("ArrayTypeSyntax");
        }

        public override void VisitNullableType(NullableTypeSyntax node)
        {
            SymbolCounters.Instance.Increment("NullableTypeSyntax");
        }

        public override void VisitTypeParameterList(TypeParameterListSyntax node)
        {
            SymbolCounters.Instance.Increment("TypeParameterListSyntax");
        }

        public override void VisitTypeParameterConstraintClause(TypeParameterConstraintClauseSyntax node)
        {
            SymbolCounters.Instance.Increment("TypeParameterConstraintClauseSyntax");
        }

        public override void VisitExplicitInterfaceSpecifier(ExplicitInterfaceSpecifierSyntax node)
        {
            SymbolCounters.Instance.Increment("ExplicitInterfaceSpecifierSyntax");
        }

        public override void VisitGenericName(GenericNameSyntax node)
        {
            SymbolCounters.Instance.Increment("GenericNameSyntax");
        }

        public override void VisitBlock(BlockSyntax node)
        {
        }

        public override void VisitArrowExpressionClause(ArrowExpressionClauseSyntax node)
        {
        }

        public override void VisitDocumentationCommentTrivia(DocumentationCommentTriviaSyntax node)
        {
            comment = commentCompiler.Compile(node);
        }
    }
}
