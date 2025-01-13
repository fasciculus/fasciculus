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

    public class Syntax
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

        private readonly SyntaxKind[] AliasQualifiedNameKinds
            = [SyntaxKind.IdentifierName];

        private readonly SyntaxKind[] ArrayRankSpecifierKinds
            = [SyntaxKind.OmittedArraySizeExpression];

        private readonly SyntaxKind[] ArrayTypeKinds
            = [SyntaxKind.ArrayRankSpecifier, SyntaxKind.GenericName, SyntaxKind.IdentifierName, SyntaxKind.PredefinedType];

        private readonly SyntaxKind[] AttributeListKinds
            = [SyntaxKind.Attribute, SyntaxKind.AttributeTargetSpecifier];

        private readonly SyntaxKind[] BaseListKinds
            = [SyntaxKind.SimpleBaseType];

        private readonly SyntaxKind[] ClassDeclarationKinds
            = [SyntaxKind.AttributeList, SyntaxKind.BaseList, SyntaxKind.ClassDeclaration, SyntaxKind.FieldDeclaration,
                SyntaxKind.MethodDeclaration, SyntaxKind.TypeParameterList];

        private readonly SyntaxKind[] CompilationUnitKinds
            = [SyntaxKind.AttributeList, SyntaxKind.NamespaceDeclaration, SyntaxKind.UsingDirective];

        private readonly SyntaxKind[] EnumDeclarationKinds
            = [SyntaxKind.EnumMemberDeclaration];

        private readonly SyntaxKind[] FieldDeclarationKinds
            = [SyntaxKind.VariableDeclaration];

        private readonly SyntaxKind[] GenericNameKinds
            = [SyntaxKind.TypeArgumentList];

        private readonly SyntaxKind[] ExplicitInterfaceSpecifierKinds
            = [SyntaxKind.GenericName, SyntaxKind.IdentifierName];

        private readonly SyntaxKind[] InterfaceDeclarationKinds
            = [SyntaxKind.BaseList, SyntaxKind.MethodDeclaration, SyntaxKind.TypeParameterList];

        private readonly SyntaxKind[] MethodDeclarationKinds
            = [SyntaxKind.ArrayType, SyntaxKind.ArrowExpressionClause, SyntaxKind.AttributeList, SyntaxKind.Block,
                SyntaxKind.ExplicitInterfaceSpecifier, SyntaxKind.GenericName, SyntaxKind.IdentifierName, SyntaxKind.NullableType,
                SyntaxKind.ParameterList, SyntaxKind.PredefinedType, SyntaxKind.TypeParameterList];

        private readonly SyntaxKind[] NamespaceDeclarationKinds
            = [SyntaxKind.ClassDeclaration, SyntaxKind.EnumDeclaration, SyntaxKind.InterfaceDeclaration, SyntaxKind.QualifiedName];

        private readonly SyntaxKind[] NullableTypeKinds
            = [SyntaxKind.GenericName, SyntaxKind.IdentifierName, SyntaxKind.PredefinedType];

        private readonly SyntaxKind[] ParameterKinds
            = [SyntaxKind.ArrayType, SyntaxKind.GenericName, SyntaxKind.IdentifierName, SyntaxKind.NullableType, SyntaxKind.PointerType,
                SyntaxKind.PredefinedType];

        private readonly SyntaxKind[] ParameterListKinds
            = [SyntaxKind.Parameter];

        private readonly SyntaxKind[] QualifiedNameKinds
            = [SyntaxKind.AliasQualifiedName, SyntaxKind.IdentifierName, SyntaxKind.QualifiedName];

        private readonly SyntaxKind[] SimpleBaseTypeKinds
            = [SyntaxKind.GenericName, SyntaxKind.IdentifierName];

        private readonly SyntaxKind[] TypeArgumentListKinds
            = [SyntaxKind.GenericName, SyntaxKind.IdentifierName, SyntaxKind.NullableType, SyntaxKind.PredefinedType];

        private readonly SyntaxKind[] TypeParameterListKinds
            = [SyntaxKind.TypeParameter];

        private readonly SyntaxKind[] UsingDirectiveKinds
            = [SyntaxKind.IdentifierName, SyntaxKind.QualifiedName];

        private readonly SyntaxKind[] VariableDeclarationKinds
            = [SyntaxKind.ArrayType, SyntaxKind.GenericName, SyntaxKind.IdentifierName, SyntaxKind.NullableType, SyntaxKind.PredefinedType,
                SyntaxKind.VariableDeclarator];

        private void AddInfos()
        {
            infos.Add(SyntaxKind.AliasQualifiedName, new(AliasQualifiedNameKinds));
            infos.Add(SyntaxKind.ArrayRankSpecifier, new(ArrayRankSpecifierKinds));
            infos.Add(SyntaxKind.ArrayType, new(ArrayTypeKinds));
            infos.Add(SyntaxKind.AttributeList, new(AttributeListKinds));
            infos.Add(SyntaxKind.BaseList, new(BaseListKinds));
            infos.Add(SyntaxKind.ClassDeclaration, new(ClassDeclarationKinds));
            infos.Add(SyntaxKind.CompilationUnit, new(CompilationUnitKinds));
            infos.Add(SyntaxKind.EnumDeclaration, new(EnumDeclarationKinds));
            infos.Add(SyntaxKind.ExplicitInterfaceSpecifier, new(ExplicitInterfaceSpecifierKinds));
            infos.Add(SyntaxKind.FieldDeclaration, new(FieldDeclarationKinds));
            infos.Add(SyntaxKind.GenericName, new(GenericNameKinds));
            infos.Add(SyntaxKind.InterfaceDeclaration, new(InterfaceDeclarationKinds));
            infos.Add(SyntaxKind.MethodDeclaration, new(MethodDeclarationKinds));
            infos.Add(SyntaxKind.NamespaceDeclaration, new(NamespaceDeclarationKinds));
            infos.Add(SyntaxKind.NullableType, new(NullableTypeKinds));
            infos.Add(SyntaxKind.Parameter, new(ParameterKinds));
            infos.Add(SyntaxKind.ParameterList, new(ParameterListKinds));
            infos.Add(SyntaxKind.QualifiedName, new(QualifiedNameKinds));
            infos.Add(SyntaxKind.SimpleBaseType, new(SimpleBaseTypeKinds));
            infos.Add(SyntaxKind.TypeArgumentList, new(TypeArgumentListKinds));
            infos.Add(SyntaxKind.TypeParameterList, new(TypeParameterListKinds));
            infos.Add(SyntaxKind.UsingDirective, new(UsingDirectiveKinds));
            infos.Add(SyntaxKind.VariableDeclaration, new(VariableDeclarationKinds));
        }
    }
}
