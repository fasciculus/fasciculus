using Fasciculus.CodeAnalysis.Compilers.Builders;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public partial class CompilationUnitCompiler
    {
        protected readonly Stack<ITypeReceiver> typeReceivers = [];

        public override void VisitBaseList(BaseListSyntax node)
        {
            // BaseList
            // : SimpleBaseType+

            nodeDebugger.Add(node);

            base.VisitBaseList(node);
        }

        public override void VisitSimpleBaseType(SimpleBaseTypeSyntax node)
        {
            // SimpleBaseType
            // : IdentifierName
            // | GenericName

            nodeDebugger.Add(node);

            base.VisitSimpleBaseType(node);
        }

        protected virtual string GetTypeName(TypeSyntax type)
        {
            if (type is PredefinedTypeSyntax predefined) return GetPredefinedTypeName(predefined);
            if (type is IdentifierNameSyntax identifier) return GetIdentifierTypeName(identifier);
            if (type is GenericNameSyntax generic) return GetGenericTypeName(generic);
            if (type is NullableTypeSyntax nullable) return GetNullableTypeName(nullable);

            return string.Empty;
        }

        protected virtual string GetPredefinedTypeName(PredefinedTypeSyntax type)
        {
            return type.Keyword.ToString();
        }

        protected virtual string GetIdentifierTypeName(IdentifierNameSyntax identifier)
        {
            return identifier.Identifier.ValueText;
        }

        protected virtual string GetGenericTypeName(GenericNameSyntax type)
        {
            string name = type.Identifier.ValueText;
            TypeSyntax[] args = [.. type.TypeArgumentList.Arguments];
            string[] names = [.. args.Select(GetTypeName)];

            return $"{name}<{string.Join(",", names)}>";
        }

        protected virtual string GetNullableTypeName(NullableTypeSyntax nullable)
        {
            return GetTypeName(nullable.ElementType) + "?";
        }
    }
}