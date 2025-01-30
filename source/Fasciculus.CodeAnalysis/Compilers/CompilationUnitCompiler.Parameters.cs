using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Fasciculus.CodeAnalysis.Compilers
{
    internal partial class CompilationUnitCompiler
    {
        public override void VisitBracketedParameterList(BracketedParameterListSyntax node)
        {
            // BracketedParameterList: Parameter

            nodeDebugger.Add(node);

            //base.VisitBracketedParameterList(node);
        }

        public override void VisitParameterList(ParameterListSyntax node)
        {
            // ParameterList: Parameter*

            nodeDebugger.Add(node);

            base.VisitParameterList(node);
        }
    }
}