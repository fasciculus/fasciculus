using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public partial class CompileUnitCompiler
    {
        public override void VisitAccessorDeclaration(AccessorDeclarationSyntax node)
        {
            // covers GetAccessorDeclaration, SetAccessorDeclaration, InitAccessorDeclaration, AddAccessorDeclaration,
            //  RemoveAccessorDeclaration, UnknownAccessorDeclaration
            //
            // GetAccessorDeclaration: ArrowExpressionClause?
            // SetAccessorDeclaration:

            nodeDebugger.Add(node);

            base.VisitAccessorDeclaration(node);
        }

        public override void VisitAccessorList(AccessorListSyntax node)
        {
            // AccessorList: GetAccessorDeclaration? SetAccessorDeclaration?

            nodeDebugger.Add(node);

            base.VisitAccessorList(node);
        }

        public override void VisitAliasQualifiedName(AliasQualifiedNameSyntax node)
        {
            // AliasQualifiedName
            // : IdentifierName IdentifierName

            nodeDebugger.Add(node);

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

            nodeDebugger.Add(node);

            base.VisitArgument(node);
        }

        public override void VisitArgumentList(ArgumentListSyntax node)
        {
            // ArgumentList: Argument*

            nodeDebugger.Add(node);

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

            nodeDebugger.Add(node);

            base.VisitArrayRankSpecifier(node);
        }

        public override void VisitArrayType(ArrayTypeSyntax node)
        {
            // ArrayType
            // : (IdentifierName | GenericName | PredefinedType) ArrayRankSpecifier

            nodeDebugger.Add(node);

            base.VisitArrayType(node);
        }

        public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            nodeDebugger.Add(node);

            base.VisitAssignmentExpression(node);
        }

        public override void VisitBracketedParameterList(BracketedParameterListSyntax node)
        {
            // BracketedParameterList: Parameter

            nodeDebugger.Add(node);

            base.VisitBracketedParameterList(node);
        }

        public override void VisitClassOrStructConstraint(ClassOrStructConstraintSyntax node)
        {
            // ClassConstraint or StructConstraint
            // Leaf

            nodeDebugger.Add(node);

            base.VisitClassOrStructConstraint(node);
        }

        public override void VisitCollectionExpression(CollectionExpressionSyntax node)
        {
            // CollectionExpression:

            nodeDebugger.Add(node);

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

            nodeDebugger.Add(node);

            base.VisitEqualsValueClause(node);
        }

        public override void VisitExplicitInterfaceSpecifier(ExplicitInterfaceSpecifierSyntax node)
        {
            // ExplicitInterfaceSpecifier
            // : IdentifierName
            // | GenericName

            nodeDebugger.Add(node);

            base.VisitExplicitInterfaceSpecifier(node);
        }

        public override void VisitGenericName(GenericNameSyntax node)
        {
            // GenericName
            // : TypeArgumentList

            nodeDebugger.Add(node);

            base.VisitGenericName(node);
        }

        public override void VisitImplicitObjectCreationExpression(ImplicitObjectCreationExpressionSyntax node)
        {
            // ImplicitObjectCreationExpression: ArgumentList

            nodeDebugger.Add(node);

            base.VisitImplicitObjectCreationExpression(node);
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            // InvocationExpression: SimpleMemberAccessExpression ArgumentList

            nodeDebugger.Add(node);

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

            nodeDebugger.Add(node);

            base.VisitLiteralExpression(node);
        }

        public override void VisitNullableType(NullableTypeSyntax node)
        {
            // NullableType
            // : IdentifierName
            // | GenericName
            // | PredefinedType

            nodeDebugger.Add(node);

            base.VisitNullableType(node);
        }

        public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            // ObjectCreationExpression: GenericName ArgumentList

            nodeDebugger.Add(node);

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

            nodeDebugger.Add(node);

            base.VisitParameter(node);
        }

        public override void VisitParameterList(ParameterListSyntax node)
        {
            // ParameterList: Parameter*

            nodeDebugger.Add(node);

            base.VisitParameterList(node);
        }

        public override void VisitPredefinedType(PredefinedTypeSyntax node)
        {
            // Leaf

            nodeDebugger.Add(node);

            base.VisitPredefinedType(node);
        }

        public override void VisitQualifiedName(QualifiedNameSyntax node)
        {
            // QualifiedName
            // : IdentifierName IdentifierName
            // | QualifiedName IdentifierName
            // | AliasQualifiedName IdentifierName

            nodeDebugger.Add(node);

            base.VisitQualifiedName(node);
        }

        public override void VisitTypeArgumentList(TypeArgumentListSyntax node)
        {
            // TypeArgumentList
            // : (IdentifierName | PredefinedType)+
            // | GenericName
            // | NullableType

            nodeDebugger.Add(node);

            base.VisitTypeArgumentList(node);
        }

        public override void VisitTypeConstraint(TypeConstraintSyntax node)
        {
            nodeDebugger.Add(node);

            base.VisitTypeConstraint(node);
        }

        public override void VisitTypeParameterConstraintClause(TypeParameterConstraintClauseSyntax node)
        {
            // TypeParameterConstraintClause
            // : IdentifierName ClassConstraint
            // | IdentifierName TypeConstraint+

            nodeDebugger.Add(node);

            base.VisitTypeParameterConstraintClause(node);
        }

        public override void VisitTypeParameterList(TypeParameterListSyntax node)
        {
            // TypeParameterList: TypeParameter+

            nodeDebugger.Add(node);

            base.VisitTypeParameterList(node);
        }

        public override void VisitVariableDeclaration(VariableDeclarationSyntax node)
        {
            // VariableDeclaration
            // : (IdentifierName | GenericName | PredefinedType | ArrayType | NullableType) VariableDeclarator

            nodeDebugger.Add(node);

            base.VisitVariableDeclaration(node);
        }

    }
}