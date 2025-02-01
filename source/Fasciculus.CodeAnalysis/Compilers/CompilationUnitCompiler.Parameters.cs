using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Fasciculus.CodeAnalysis.Compilers
{
    internal partial class CompilationUnitCompiler
    {
        public override void VisitBracketedParameterList(BracketedParameterListSyntax node)
        {
            // BracketedParameterList: Parameter

            NodeDebugger.Add(node);

            //base.VisitBracketedParameterList(node);
        }

        public override void VisitParameterList(ParameterListSyntax node)
        {
            // ParameterList: Parameter*

            NodeDebugger.Add(node);

            base.VisitParameterList(node);
        }
    }
}