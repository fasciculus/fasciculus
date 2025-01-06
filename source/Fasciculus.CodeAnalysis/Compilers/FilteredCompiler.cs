using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class FilteredCompiler : CSharpSyntaxWalker
    {
        private SortedSet<SyntaxKind> kinds;

        public FilteredCompiler(IEnumerable<SyntaxKind> kinds)
        {
            this.kinds = new(kinds);
        }

        public override void Visit(SyntaxNode? node)
        {
            if (node is not null)
            {
                SyntaxKind kind = node.Kind();

                if (kinds.Contains(kind))
                {
                    base.Visit(node);
                }
            }
        }
    }
}
