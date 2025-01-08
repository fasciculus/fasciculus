using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class FilteredCompiler : CSharpSyntaxWalker
    {
        private SortedSet<SyntaxKind> handledSymbols;

        public FilteredCompiler(IEnumerable<SyntaxKind> handledSymbols, SyntaxWalkerDepth depth = SyntaxWalkerDepth.Node)
            : base(depth)
        {
            this.handledSymbols = new(handledSymbols);

            UnhandledSymbols.Instance.Handled(GetType(), handledSymbols);
        }

        public override void Visit(SyntaxNode? node)
        {
            if (node is not null)
            {
                SyntaxKind kind = node.Kind();

                UnhandledSymbols.Instance.Used(GetType(), kind);

                if (handledSymbols.Contains(kind))
                {
                    base.Visit(node);
                }
            }
        }
    }
}
