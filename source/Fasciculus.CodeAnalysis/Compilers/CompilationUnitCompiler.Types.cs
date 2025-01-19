using Fasciculus.CodeAnalysis.Compilers.Builders;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public partial class CompilationUnitCompiler
    {
        protected readonly Stack<ITypeReceiver> typeReceivers = [];

        public override void VisitBaseList(BaseListSyntax node)
        {
            // BaseList
            // : SimpleBaseType+

            NodeDebugger.Add(node);

            base.VisitBaseList(node);
        }

        public override void VisitSimpleBaseType(SimpleBaseTypeSyntax node)
        {
            // SimpleBaseType
            // : IdentifierName
            // | GenericName

            NodeDebugger.Add(node);

            base.VisitSimpleBaseType(node);
        }

        protected virtual string GetTypeName(TypeSyntax type)
        {
            if (type is PredefinedTypeSyntax predefined) return GetTypeName(predefined);
            if (type is GenericNameSyntax generic) return GetTypeName(generic);

            return string.Empty;
        }

        protected virtual string GetTypeName(PredefinedTypeSyntax type)
        {
            return type.Keyword.ToString();
        }

        protected virtual string GetTypeName(GenericNameSyntax type)
        {
            TypeArgumentListSyntax args = type.TypeArgumentList;

            return string.Empty;
        }
    }
}