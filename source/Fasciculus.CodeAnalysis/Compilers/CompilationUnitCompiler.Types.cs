using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Fasciculus.CodeAnalysis.Compilers
{
    internal partial class CompilationUnitCompiler
    {
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
    }
}