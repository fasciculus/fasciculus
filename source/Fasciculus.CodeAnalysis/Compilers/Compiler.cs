using Fasciculus.CodeAnalysis.Compilers.Builders;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public partial class Compiler : ICompiler
    {
        private readonly CompilerContext context;

        private readonly Stack<CommentBuilder> commentBuilders = [];

        public CommentBuilder CommentBuilder => commentBuilders.Peek();

        public Compiler(CompilerContext context)
        {
            this.context = context;
        }

        public virtual void Compile(CompilationUnitSyntax compilationUnit)
        {
            Walker walker = new(this, context.NodeDebugger);

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
