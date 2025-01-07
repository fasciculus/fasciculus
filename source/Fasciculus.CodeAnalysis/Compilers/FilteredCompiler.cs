using Fasciculus.CodeAnalysis.Support;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class FilteredCompiler : CSharpSyntaxWalker
    {
        private SortedSet<SyntaxKind> acceptedKinds;

        public FilteredCompiler(IEnumerable<SyntaxKind> acceptedKinds, SyntaxWalkerDepth depth = SyntaxWalkerDepth.Node)
            : base(depth)
        {
            this.acceptedKinds = new(acceptedKinds);

            NodeKindReporter.Instance.Add(GetType(), acceptedKinds);
        }

        public override void Visit(SyntaxNode? node)
        {
            if (node is not null)
            {
                SyntaxKind kind = node.Kind();

                NodeKindReporter.Instance.Used(GetType(), kind);

                if (acceptedKinds.Contains(kind))
                {
                    base.Visit(node);
                }
            }
        }
    }
}
