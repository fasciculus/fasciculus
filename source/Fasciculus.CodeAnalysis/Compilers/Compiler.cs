using Fasciculus.CodeAnalysis.Compilers.Builders;
using Fasciculus.CodeAnalysis.Debugging;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public partial class Compiler : ICompiler
    {
        private readonly INodeDebugger nodeDebugger;

        private readonly Stack<CommentBuilder> commentBuilders = [];

        public CommentBuilder CommentBuilder => commentBuilders.Peek();

        public Compiler(INodeDebugger nodeDebugger)
        {
            this.nodeDebugger = nodeDebugger;
        }

        public virtual void Compile(CompilationUnitSyntax compilationUnit)
        {
            Walker walker = new(this, nodeDebugger);

            compilationUnit.Accept(walker);
        }

        public void PushComment()
        {
            commentBuilders.Push(CommentBuilder.Create());
        }

        public void PopComment()
        {
            commentBuilders.Pop();
        }
    }
}
