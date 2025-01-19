using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public partial class CompilationUnitCompiler
    {
        public override void VisitAliasQualifiedName(AliasQualifiedNameSyntax node)
        {
            // AliasQualifiedName
            // : IdentifierName IdentifierName

            NodeDebugger.Add(node);

            base.VisitAliasQualifiedName(node);
        }

        public override void VisitArgument(ArgumentSyntax node)
        {
            // Argument
            // : IdentifierName
            // | SimpleMemberAccessExpression
            // | NumericLiteralExpression
            // | NullLiteralExpression
            // | CollectionExpression
            // | InvocationExpression
            // | ObjectCreationExpression
            // plus maybe more

            NodeDebugger.Add(node);

            base.VisitArgument(node);
        }

        public override void VisitArgumentList(ArgumentListSyntax node)
        {
            // ArgumentList: Argument*

            NodeDebugger.Add(node);

            base.VisitArgumentList(node);
        }

        public override void VisitArrayRankSpecifier(ArrayRankSpecifierSyntax node)
        {
            // ArrayRankSpecifier
            // : IdentifierName
            // | InvocationExpression
            // | OmittedArraySizeExpression
            // | AddExpression
            // | NumericLiteralExpression
            //
            // only OmittedArraySizeExpression occurs on fields.

            NodeDebugger.Add(node);

            base.VisitArrayRankSpecifier(node);
        }

        public override void VisitArrayType(ArrayTypeSyntax node)
        {
            // ArrayType
            // : (IdentifierName | GenericName | PredefinedType) ArrayRankSpecifier

            NodeDebugger.Add(node);

            base.VisitArrayType(node);
        }

        public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            NodeDebugger.Add(node);

            base.VisitAssignmentExpression(node);
        }

        public override void VisitBracketedParameterList(BracketedParameterListSyntax node)
        {
            // BracketedParameterList: Parameter

            NodeDebugger.Add(node);

            base.VisitBracketedParameterList(node);
        }

        public override void VisitClassOrStructConstraint(ClassOrStructConstraintSyntax node)
        {
            // ClassConstraint or StructConstraint
            // Leaf

            NodeDebugger.Add(node);

            base.VisitClassOrStructConstraint(node);
        }

        public override void VisitCollectionExpression(CollectionExpressionSyntax node)
        {
            // CollectionExpression:

            NodeDebugger.Add(node);

            base.VisitCollectionExpression(node);
        }

        public override void VisitEqualsValueClause(EqualsValueClauseSyntax node)
        {
            // EqualsValueClause
            // : ImplicitObjectCreationExpression
            // | FalseLiteralExpression
            // | NullLiteralExpression
            // | DefaultLiteralExpression
            // | CollectionExpression

            NodeDebugger.Add(node);

            base.VisitEqualsValueClause(node);
        }

        public override void VisitExplicitInterfaceSpecifier(ExplicitInterfaceSpecifierSyntax node)
        {
            // ExplicitInterfaceSpecifier
            // : IdentifierName
            // | GenericName

            NodeDebugger.Add(node);

            base.VisitExplicitInterfaceSpecifier(node);
        }

        public override void VisitGenericName(GenericNameSyntax node)
        {
            // GenericName
            // : TypeArgumentList

            NodeDebugger.Add(node);

            base.VisitGenericName(node);
        }

        public override void VisitImplicitObjectCreationExpression(ImplicitObjectCreationExpressionSyntax node)
        {
            // ImplicitObjectCreationExpression: ArgumentList

            NodeDebugger.Add(node);

            base.VisitImplicitObjectCreationExpression(node);
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            // InvocationExpression: SimpleMemberAccessExpression ArgumentList

            NodeDebugger.Add(node);

            base.VisitInvocationExpression(node);
        }

        public override void VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            // covers ArgListExpression, NumericLiteralExpression, StringLiteralExpression, Utf8StringLiteralExpression
            //  CharacterLiteralExpression, TrueLiteralExpression, FalseLiteralExpression, NullLiteralExpression, DefaultLiteralExpression
            //
            // DefaultLiteralExpression:
            // FalseLiteralExpression:
            // NullLiteralExpression:

            NodeDebugger.Add(node);

            base.VisitLiteralExpression(node);
        }

        public override void VisitNullableType(NullableTypeSyntax node)
        {
            // NullableType
            // : IdentifierName
            // | GenericName
            // | PredefinedType

            NodeDebugger.Add(node);

            base.VisitNullableType(node);
        }

        public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            // ObjectCreationExpression: GenericName ArgumentList

            NodeDebugger.Add(node);

            base.VisitObjectCreationExpression(node);
        }

        public override void VisitParameter(ParameterSyntax node)
        {
            // Parameter
            // : IdentifierName EqualsValueClause?
            // | GenericName
            // | PredefinedType EqualsValueClause?
            // | ArrayType
            // | PointerType
            // | NullableType EqualsValueClause?

            NodeDebugger.Add(node);

            base.VisitParameter(node);
        }

        public override void VisitParameterList(ParameterListSyntax node)
        {
            // ParameterList: Parameter*

            NodeDebugger.Add(node);

            base.VisitParameterList(node);
        }

        public override void VisitPredefinedType(PredefinedTypeSyntax node)
        {
            // Leaf

            NodeDebugger.Add(node);

            base.VisitPredefinedType(node);
        }

        public override void VisitQualifiedName(QualifiedNameSyntax node)
        {
            // QualifiedName
            // : IdentifierName IdentifierName
            // | QualifiedName IdentifierName
            // | AliasQualifiedName IdentifierName

            NodeDebugger.Add(node);

            base.VisitQualifiedName(node);
        }

        public override void VisitTypeArgumentList(TypeArgumentListSyntax node)
        {
            // TypeArgumentList
            // : (IdentifierName | PredefinedType)+
            // | GenericName
            // | NullableType

            NodeDebugger.Add(node);

            base.VisitTypeArgumentList(node);
        }

        public override void VisitTypeConstraint(TypeConstraintSyntax node)
        {
            NodeDebugger.Add(node);

            base.VisitTypeConstraint(node);
        }

        public override void VisitTypeParameterConstraintClause(TypeParameterConstraintClauseSyntax node)
        {
            // TypeParameterConstraintClause
            // : IdentifierName ClassConstraint
            // | IdentifierName TypeConstraint+

            NodeDebugger.Add(node);

            base.VisitTypeParameterConstraintClause(node);
        }

        public override void VisitTypeParameterList(TypeParameterListSyntax node)
        {
            // TypeParameterList: TypeParameter+

            NodeDebugger.Add(node);

            base.VisitTypeParameterList(node);
        }

        public override void VisitVariableDeclaration(VariableDeclarationSyntax node)
        {
            // VariableDeclaration
            // : (IdentifierName | GenericName | PredefinedType | ArrayType | NullableType) VariableDeclarator

            NodeDebugger.Add(node);

            base.VisitVariableDeclaration(node);
        }

    }
}