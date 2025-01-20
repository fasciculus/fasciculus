using Fasciculus.CodeAnalysis.Debugging;
using Fasciculus.CodeAnalysis.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class CompilerBase : CSharpSyntaxWalker
    {
        protected readonly ModifiersCompiler modifiersCompiler;

        protected readonly bool includeNonAccessible;

        public INodeDebugger nodeDebugger { get; }

        public CompilerBase(CompilerContext context, SyntaxWalkerDepth depth)
            : base(depth)
        {
            modifiersCompiler = new(context);
            includeNonAccessible = context.IncludeNonAccessible;

            nodeDebugger = context.Debuggers.NodeDebugger;
        }

        protected bool IsIncluded(SymbolModifiers modifiers)
            => includeNonAccessible || modifiers.IsAccessible;
    }
}
