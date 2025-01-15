using Fasciculus.CodeAnalysis.Debugging;
using Fasciculus.Collections;
using Fasciculus.Threading.Synchronization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Support
{
    public class SyntaxInfo
    {
        private readonly SortedSet<SyntaxKind> handled;
        private readonly SortedSet<SyntaxKind> used = [];

        public SyntaxInfo(IEnumerable<SyntaxKind> handled)
        {
            this.handled = new(handled);
        }

        public void AddUsed(IEnumerable<SyntaxKind> kinds)
        {
            kinds.Apply(k => { used.Add(k); });
        }

        public SortedSet<SyntaxKind> GetUnhandled()
        {
            SortedSet<SyntaxKind> unhandled = new(used);

            unhandled.ExceptWith(handled);

            return unhandled;
        }
    }

    public class Syntax : INodeDebugger
    {
        public static readonly Syntax Instance = new();

        private readonly TaskSafeMutex mutex = new();

        private readonly Dictionary<SyntaxKind, SyntaxInfo> infos = [];

        private Syntax()
        {
            AddInfos();
        }

        public void Add(SyntaxNode node)
        {
            using Locker locker = Locker.Lock(mutex);

            SyntaxKind kind = node.Kind();
            IEnumerable<SyntaxKind> used = node.ChildNodes().Select(c => c.Kind());

            if (!infos.TryGetValue(kind, out SyntaxInfo? info))
            {
                info = new([]);
                infos.Add(kind, info);
            }

            info.AddUsed(used);
        }

        public Dictionary<SyntaxKind, SortedSet<SyntaxKind>> GetUnhandled()
        {
            using Locker locker = Locker.Lock(mutex);

            Dictionary<SyntaxKind, SortedSet<SyntaxKind>> result = [];

            foreach (var kvp in infos)
            {
                SortedSet<SyntaxKind> unhandled = kvp.Value.GetUnhandled();

                if (unhandled.Count > 0)
                {
                    result.Add(kvp.Key, unhandled);
                }
            }

            return result;
        }

        private readonly SyntaxKind[] AccessorListKinds
            = [SyntaxKind.GetAccessorDeclaration, SyntaxKind.SetAccessorDeclaration];

        private readonly SyntaxKind[] AliasQualifiedNameKinds
            = [SyntaxKind.IdentifierName];

        private readonly SyntaxKind[] ArgumentKinds
            = [SyntaxKind.CollectionExpression, SyntaxKind.IdentifierName, SyntaxKind.InvocationExpression, SyntaxKind.NullLiteralExpression,
                SyntaxKind.NumericLiteralExpression, SyntaxKind.ObjectCreationExpression, SyntaxKind.SimpleMemberAccessExpression];

        private readonly SyntaxKind[] ArgumentListKinds
            = [SyntaxKind.Argument];

        private readonly SyntaxKind[] ArrayRankSpecifierKinds
            = [SyntaxKind.OmittedArraySizeExpression];

        private readonly SyntaxKind[] ArrayTypeKinds
            = [SyntaxKind.ArrayRankSpecifier, SyntaxKind.GenericName, SyntaxKind.IdentifierName, SyntaxKind.PredefinedType];

        private readonly SyntaxKind[] AttributeListKinds
            = [SyntaxKind.Attribute, SyntaxKind.AttributeTargetSpecifier];

        private readonly SyntaxKind[] BaseListKinds
            = [SyntaxKind.SimpleBaseType];

        private readonly SyntaxKind[] BracketedParameterListKinds
            = [SyntaxKind.Parameter];

        private readonly SyntaxKind[] ClassDeclarationKinds
            = [SyntaxKind.AttributeList, SyntaxKind.BaseList, SyntaxKind.ClassDeclaration, SyntaxKind.ConstructorDeclaration,
                SyntaxKind.ConversionOperatorDeclaration, SyntaxKind.DestructorDeclaration, SyntaxKind.EventFieldDeclaration,
                SyntaxKind.FieldDeclaration, SyntaxKind.IndexerDeclaration, SyntaxKind.MethodDeclaration, SyntaxKind.OperatorDeclaration,
                SyntaxKind.PropertyDeclaration, SyntaxKind.TypeParameterList, SyntaxKind.TypeParameterConstraintClause];

        private readonly SyntaxKind[] CompilationUnitKinds
            = [SyntaxKind.AttributeList, SyntaxKind.NamespaceDeclaration, SyntaxKind.UsingDirective];

        private readonly SyntaxKind[] ConstructorDeclarationKinds
            = [SyntaxKind.ParameterList, SyntaxKind.BaseConstructorInitializer, SyntaxKind.Block, SyntaxKind.ThisConstructorInitializer];

        private readonly SyntaxKind[] ConversionOperatorDeclarationKinds
            = [SyntaxKind.ArrowExpressionClause, SyntaxKind.IdentifierName, SyntaxKind.ParameterList];

        private readonly SyntaxKind[] DestructorDeclarationKinds
            = [SyntaxKind.ParameterList, SyntaxKind.Block];

        private readonly SyntaxKind[] EnumDeclarationKinds
            = [SyntaxKind.EnumMemberDeclaration];

        private readonly SyntaxKind[] EqualsValueClauseKinds
            = [SyntaxKind.CollectionExpression, SyntaxKind.DefaultLiteralExpression, SyntaxKind.FalseLiteralExpression,
                SyntaxKind.ImplicitObjectCreationExpression, SyntaxKind.NullLiteralExpression];

        private readonly SyntaxKind[] EventFieldDeclarationKinds
            = [SyntaxKind.VariableDeclaration];

        private readonly SyntaxKind[] ExplicitInterfaceSpecifierKinds
            = [SyntaxKind.GenericName, SyntaxKind.IdentifierName];

        private readonly SyntaxKind[] FieldDeclarationKinds
            = [SyntaxKind.VariableDeclaration];

        private readonly SyntaxKind[] GenericNameKinds
            = [SyntaxKind.TypeArgumentList];

        private readonly SyntaxKind[] GetAccessorDeclarationKinds
            = [SyntaxKind.ArrowExpressionClause];

        private readonly SyntaxKind[] ImplicitObjectCreationExpressionKinds
            = [SyntaxKind.ArgumentList];

        private readonly SyntaxKind[] IndexerDeclarationKinds
            = [SyntaxKind.ArrowExpressionClause, SyntaxKind.BracketedParameterList, SyntaxKind.PredefinedType];

        private readonly SyntaxKind[] InterfaceDeclarationKinds
            = [SyntaxKind.BaseList, SyntaxKind.MethodDeclaration, SyntaxKind.TypeParameterConstraintClause, SyntaxKind.TypeParameterList];

        private readonly SyntaxKind[] InvocationExpressionKinds
            = [SyntaxKind.ArgumentList, SyntaxKind.SimpleMemberAccessExpression];

        private readonly SyntaxKind[] MethodDeclarationKinds
            = [SyntaxKind.ArrayType, SyntaxKind.ArrowExpressionClause, SyntaxKind.AttributeList, SyntaxKind.Block,
                SyntaxKind.ExplicitInterfaceSpecifier, SyntaxKind.GenericName, SyntaxKind.IdentifierName, SyntaxKind.NullableType,
                SyntaxKind.ParameterList, SyntaxKind.PredefinedType, SyntaxKind.TypeParameterConstraintClause, SyntaxKind.TypeParameterList];

        private readonly SyntaxKind[] NamespaceDeclarationKinds
            = [SyntaxKind.ClassDeclaration, SyntaxKind.EnumDeclaration, SyntaxKind.InterfaceDeclaration, SyntaxKind.QualifiedName];

        private readonly SyntaxKind[] NullableTypeKinds
            = [SyntaxKind.GenericName, SyntaxKind.IdentifierName, SyntaxKind.PredefinedType];

        private readonly SyntaxKind[] OperatorDeclarationKinds
            = [SyntaxKind.ArrowExpressionClause, SyntaxKind.AttributeList, SyntaxKind.IdentifierName, SyntaxKind.ParameterList];

        private readonly SyntaxKind[] ObjectCreationExpressionKinds
            = [SyntaxKind.ArgumentList, SyntaxKind.GenericName];

        private readonly SyntaxKind[] ParameterKinds
            = [SyntaxKind.ArrayType, SyntaxKind.EqualsValueClause, SyntaxKind.GenericName, SyntaxKind.IdentifierName, SyntaxKind.NullableType,
                SyntaxKind.PointerType, SyntaxKind.PredefinedType];

        private readonly SyntaxKind[] ParameterListKinds
            = [SyntaxKind.Parameter];

        private readonly SyntaxKind[] PropertyDeclarationKinds
            = [SyntaxKind.AccessorList, SyntaxKind.ArrowExpressionClause, SyntaxKind.AttributeList, SyntaxKind.EqualsValueClause,
                SyntaxKind.ExplicitInterfaceSpecifier, SyntaxKind.GenericName, SyntaxKind.IdentifierName, SyntaxKind.NullableType,
                SyntaxKind.PredefinedType];

        private readonly SyntaxKind[] QualifiedNameKinds
            = [SyntaxKind.AliasQualifiedName, SyntaxKind.IdentifierName, SyntaxKind.QualifiedName];

        private readonly SyntaxKind[] SimpleBaseTypeKinds
            = [SyntaxKind.GenericName, SyntaxKind.IdentifierName];

        private readonly SyntaxKind[] SimpleMemberAccessExpressionKinds
            = [SyntaxKind.GenericName, SyntaxKind.IdentifierName];

        private readonly SyntaxKind[] TypeArgumentListKinds
            = [SyntaxKind.GenericName, SyntaxKind.IdentifierName, SyntaxKind.NullableType, SyntaxKind.PredefinedType];

        private readonly SyntaxKind[] TypeConstraintKinds
            = [SyntaxKind.GenericName, SyntaxKind.IdentifierName];

        private readonly SyntaxKind[] TypeParameterConstraintClauseKinds
            = [SyntaxKind.IdentifierName, SyntaxKind.ClassConstraint, SyntaxKind.TypeConstraint];

        private readonly SyntaxKind[] TypeParameterListKinds
            = [SyntaxKind.TypeParameter];

        private readonly SyntaxKind[] UsingDirectiveKinds
            = [SyntaxKind.IdentifierName, SyntaxKind.QualifiedName];

        private readonly SyntaxKind[] VariableDeclarationKinds
            = [SyntaxKind.ArrayType, SyntaxKind.GenericName, SyntaxKind.IdentifierName, SyntaxKind.NullableType, SyntaxKind.PredefinedType,
                SyntaxKind.VariableDeclarator];

        private void AddInfos()
        {
            infos.Add(SyntaxKind.AccessorList, new(AccessorListKinds));
            infos.Add(SyntaxKind.AliasQualifiedName, new(AliasQualifiedNameKinds));
            infos.Add(SyntaxKind.Argument, new(ArgumentKinds));
            infos.Add(SyntaxKind.ArgumentList, new(ArgumentListKinds));
            infos.Add(SyntaxKind.ArrayRankSpecifier, new(ArrayRankSpecifierKinds));
            infos.Add(SyntaxKind.ArrayType, new(ArrayTypeKinds));
            infos.Add(SyntaxKind.AttributeList, new(AttributeListKinds));
            infos.Add(SyntaxKind.BaseList, new(BaseListKinds));
            infos.Add(SyntaxKind.BracketedParameterList, new(BracketedParameterListKinds));
            infos.Add(SyntaxKind.ClassDeclaration, new(ClassDeclarationKinds));
            infos.Add(SyntaxKind.CompilationUnit, new(CompilationUnitKinds));
            infos.Add(SyntaxKind.ConstructorDeclaration, new(ConstructorDeclarationKinds));
            infos.Add(SyntaxKind.ConversionOperatorDeclaration, new(ConversionOperatorDeclarationKinds));
            infos.Add(SyntaxKind.DestructorDeclaration, new(DestructorDeclarationKinds));
            infos.Add(SyntaxKind.EnumDeclaration, new(EnumDeclarationKinds));
            infos.Add(SyntaxKind.EqualsValueClause, new(EqualsValueClauseKinds));
            infos.Add(SyntaxKind.EventFieldDeclaration, new(EventFieldDeclarationKinds));
            infos.Add(SyntaxKind.ExplicitInterfaceSpecifier, new(ExplicitInterfaceSpecifierKinds));
            infos.Add(SyntaxKind.FieldDeclaration, new(FieldDeclarationKinds));
            infos.Add(SyntaxKind.GenericName, new(GenericNameKinds));
            infos.Add(SyntaxKind.GetAccessorDeclaration, new(GetAccessorDeclarationKinds));
            infos.Add(SyntaxKind.ImplicitObjectCreationExpression, new(ImplicitObjectCreationExpressionKinds));
            infos.Add(SyntaxKind.IndexerDeclaration, new(IndexerDeclarationKinds));
            infos.Add(SyntaxKind.InterfaceDeclaration, new(InterfaceDeclarationKinds));
            infos.Add(SyntaxKind.InvocationExpression, new(InvocationExpressionKinds));
            infos.Add(SyntaxKind.MethodDeclaration, new(MethodDeclarationKinds));
            infos.Add(SyntaxKind.NamespaceDeclaration, new(NamespaceDeclarationKinds));
            infos.Add(SyntaxKind.NullableType, new(NullableTypeKinds));
            infos.Add(SyntaxKind.ObjectCreationExpression, new(ObjectCreationExpressionKinds));
            infos.Add(SyntaxKind.OperatorDeclaration, new(OperatorDeclarationKinds));
            infos.Add(SyntaxKind.Parameter, new(ParameterKinds));
            infos.Add(SyntaxKind.ParameterList, new(ParameterListKinds));
            infos.Add(SyntaxKind.PropertyDeclaration, new(PropertyDeclarationKinds));
            infos.Add(SyntaxKind.QualifiedName, new(QualifiedNameKinds));
            infos.Add(SyntaxKind.SimpleBaseType, new(SimpleBaseTypeKinds));
            infos.Add(SyntaxKind.SimpleMemberAccessExpression, new(SimpleMemberAccessExpressionKinds));
            infos.Add(SyntaxKind.TypeArgumentList, new(TypeArgumentListKinds));
            infos.Add(SyntaxKind.TypeConstraint, new(TypeConstraintKinds));
            infos.Add(SyntaxKind.TypeParameterConstraintClause, new(TypeParameterConstraintClauseKinds));
            infos.Add(SyntaxKind.TypeParameterList, new(TypeParameterListKinds));
            infos.Add(SyntaxKind.UsingDirective, new(UsingDirectiveKinds));
            infos.Add(SyntaxKind.VariableDeclaration, new(VariableDeclarationKinds));
        }
    }
}
