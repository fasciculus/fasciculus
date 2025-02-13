using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Debugging
{
    public class SyntaxDebuggerKnownEntries : Dictionary<SyntaxKind, SyntaxDebuggerEntry>
    {
        public SyntaxDebuggerKnownEntries()
        {
            Add(SyntaxKind.AccessorList, [SyntaxKind.GetAccessorDeclaration, SyntaxKind.SetAccessorDeclaration]);

            Add(SyntaxKind.AliasQualifiedName, [SyntaxKind.IdentifierName]);

            Add(SyntaxKind.Argument, [SyntaxKind.CollectionExpression, SyntaxKind.IdentifierName, SyntaxKind.InvocationExpression,
                SyntaxKind.NullLiteralExpression, SyntaxKind.NumericLiteralExpression, SyntaxKind.ObjectCreationExpression,
                SyntaxKind.SimpleMemberAccessExpression]);

            Add(SyntaxKind.ArgumentList, [SyntaxKind.Argument]);

            Add(SyntaxKind.ArrayRankSpecifier, [SyntaxKind.OmittedArraySizeExpression]);

            Add(SyntaxKind.ArrayType, [SyntaxKind.ArrayRankSpecifier, SyntaxKind.GenericName, SyntaxKind.IdentifierName,
                SyntaxKind.PredefinedType]);

            Add(SyntaxKind.AttributeList, [SyntaxKind.Attribute, SyntaxKind.AttributeTargetSpecifier]);

            Add(SyntaxKind.BaseList, [SyntaxKind.SimpleBaseType]);

            Add(SyntaxKind.BracketedParameterList, [SyntaxKind.Parameter]);

            Add(SyntaxKind.ClassDeclaration, [SyntaxKind.AttributeList, SyntaxKind.BaseList, SyntaxKind.ClassDeclaration,
                SyntaxKind.ConstructorDeclaration, SyntaxKind.ConversionOperatorDeclaration, SyntaxKind.DestructorDeclaration,
                SyntaxKind.EventFieldDeclaration, SyntaxKind.FieldDeclaration, SyntaxKind.IndexerDeclaration, SyntaxKind.MethodDeclaration,
                SyntaxKind.OperatorDeclaration, SyntaxKind.PropertyDeclaration, SyntaxKind.TypeParameterList,
                SyntaxKind.TypeParameterConstraintClause]);

            Add(SyntaxKind.CompilationUnit, [SyntaxKind.AttributeList, SyntaxKind.NamespaceDeclaration, SyntaxKind.UsingDirective]);

            Add(SyntaxKind.ConstructorDeclaration, [SyntaxKind.ParameterList, SyntaxKind.BaseConstructorInitializer, SyntaxKind.Block,
                SyntaxKind.ThisConstructorInitializer]);

            Add(SyntaxKind.ConversionOperatorDeclaration, [SyntaxKind.ArrowExpressionClause, SyntaxKind.IdentifierName,
                SyntaxKind.ParameterList]);

            Add(SyntaxKind.DestructorDeclaration, [SyntaxKind.ParameterList, SyntaxKind.Block]);

            Add(SyntaxKind.EnumDeclaration, [SyntaxKind.EnumMemberDeclaration]);

            Add(SyntaxKind.EqualsValueClause, [SyntaxKind.CollectionExpression, SyntaxKind.ConditionalExpression,
                SyntaxKind.DefaultLiteralExpression, SyntaxKind.FalseLiteralExpression, SyntaxKind.IdentifierName,
                SyntaxKind.ImplicitObjectCreationExpression, SyntaxKind.NullLiteralExpression, SyntaxKind.SimpleMemberAccessExpression]);

            Add(SyntaxKind.EventFieldDeclaration, [SyntaxKind.VariableDeclaration]);

            Add(SyntaxKind.ExplicitInterfaceSpecifier, [SyntaxKind.GenericName, SyntaxKind.IdentifierName]);

            Add(SyntaxKind.FieldDeclaration, [SyntaxKind.VariableDeclaration]);

            Add(SyntaxKind.GenericName, [SyntaxKind.TypeArgumentList]);

            Add(SyntaxKind.GetAccessorDeclaration, [SyntaxKind.ArrowExpressionClause]);

            Add(SyntaxKind.ImplicitObjectCreationExpression, [SyntaxKind.ArgumentList]);

            Add(SyntaxKind.IndexerDeclaration, [SyntaxKind.ArrowExpressionClause, SyntaxKind.BracketedParameterList, SyntaxKind.NullableType,
                SyntaxKind.PredefinedType]);

            Add(SyntaxKind.InterfaceDeclaration, [SyntaxKind.BaseList, SyntaxKind.MethodDeclaration, SyntaxKind.TypeParameterConstraintClause,
                SyntaxKind.TypeParameterList]);

            Add(SyntaxKind.InvocationExpression, [SyntaxKind.ArgumentList, SyntaxKind.SimpleMemberAccessExpression]);

            Add(SyntaxKind.MethodDeclaration, [SyntaxKind.ArrayType, SyntaxKind.ArrowExpressionClause, SyntaxKind.AttributeList,
                SyntaxKind.Block, SyntaxKind.ExplicitInterfaceSpecifier, SyntaxKind.GenericName, SyntaxKind.IdentifierName,
                SyntaxKind.NullableType, SyntaxKind.ParameterList, SyntaxKind.PredefinedType, SyntaxKind.TypeParameterConstraintClause,
                SyntaxKind.TypeParameterList]);

            Add(SyntaxKind.NamespaceDeclaration, [SyntaxKind.ClassDeclaration, SyntaxKind.EnumDeclaration, SyntaxKind.InterfaceDeclaration,
                SyntaxKind.QualifiedName]);

            Add(SyntaxKind.NullableType, [SyntaxKind.GenericName, SyntaxKind.IdentifierName, SyntaxKind.PredefinedType]);

            Add(SyntaxKind.ObjectCreationExpression, [SyntaxKind.ArgumentList, SyntaxKind.GenericName]);

            Add(SyntaxKind.OperatorDeclaration, [SyntaxKind.ArrowExpressionClause, SyntaxKind.AttributeList, SyntaxKind.IdentifierName,
                SyntaxKind.ParameterList, SyntaxKind.PredefinedType]);

            Add(SyntaxKind.Parameter, [SyntaxKind.ArrayType, SyntaxKind.AttributeList, SyntaxKind.EqualsValueClause, SyntaxKind.GenericName,
                SyntaxKind.IdentifierName, SyntaxKind.NullableType, SyntaxKind.PointerType, SyntaxKind.PredefinedType]);

            Add(SyntaxKind.ParameterList, [SyntaxKind.Parameter]);

            Add(SyntaxKind.PropertyDeclaration, [SyntaxKind.AccessorList, SyntaxKind.ArrayType, SyntaxKind.ArrowExpressionClause,
                SyntaxKind.AttributeList, SyntaxKind.EqualsValueClause, SyntaxKind.ExplicitInterfaceSpecifier, SyntaxKind.GenericName,
                SyntaxKind.IdentifierName, SyntaxKind.NullableType, SyntaxKind.PredefinedType]);

            Add(SyntaxKind.QualifiedName, [SyntaxKind.AliasQualifiedName, SyntaxKind.IdentifierName, SyntaxKind.QualifiedName]);

            Add(SyntaxKind.SimpleBaseType, [SyntaxKind.GenericName, SyntaxKind.IdentifierName]);

            Add(SyntaxKind.SimpleMemberAccessExpression, [SyntaxKind.GenericName, SyntaxKind.IdentifierName]);

            Add(SyntaxKind.TypeArgumentList, [SyntaxKind.GenericName, SyntaxKind.IdentifierName, SyntaxKind.NullableType,
                SyntaxKind.PredefinedType]);

            Add(SyntaxKind.TypeConstraint, [SyntaxKind.GenericName, SyntaxKind.IdentifierName]);

            Add(SyntaxKind.TypeParameterConstraintClause, [SyntaxKind.IdentifierName, SyntaxKind.ClassConstraint, SyntaxKind.TypeConstraint]);

            Add(SyntaxKind.TypeParameterList, [SyntaxKind.TypeParameter]);

            Add(SyntaxKind.UsingDirective, [SyntaxKind.IdentifierName, SyntaxKind.QualifiedName]);

            Add(SyntaxKind.VariableDeclaration, [SyntaxKind.ArrayType, SyntaxKind.GenericName, SyntaxKind.IdentifierName,
                SyntaxKind.NullableType, SyntaxKind.PredefinedType, SyntaxKind.VariableDeclarator]);
        }

        private void Add(SyntaxKind kind, IEnumerable<SyntaxKind> handled)
        {
            Add(kind, new SyntaxDebuggerEntry(handled));
        }
    }
}
