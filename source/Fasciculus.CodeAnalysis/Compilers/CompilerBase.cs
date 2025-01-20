using Fasciculus.CodeAnalysis.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class CompilerBase : CSharpSyntaxWalker
    {
        protected readonly bool includeNonAccessible;

        public CompilerBase(CompilerContext context, SyntaxWalkerDepth depth)
            : base(depth)
        {
            includeNonAccessible = context.IncludeNonAccessible;
        }

        protected bool IsIncluded(SymbolModifiers modifiers)
            => includeNonAccessible || modifiers.IsAccessible;
    }
}
