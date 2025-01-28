using Fasciculus.CodeAnalysis.Debugging;
using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.CodeAnalysis.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;

namespace Fasciculus.CodeAnalysis.Compilers
{
    internal class CompilerBase : CSharpSyntaxWalker
    {
        protected readonly TargetFramework framework;

        protected readonly string package;

        protected readonly bool includeNonAccessible;

        protected readonly DirectoryInfo projectDirectory;

        //protected readonly ModifiersCompiler modifiersCompiler;

        protected INodeDebugger nodeDebugger;

        public CompilerBase(CompilerContext context, SyntaxWalkerDepth depth)
            : base(depth)
        {
            framework = context.Framework;
            package = context.Project.AssemblyName;

            includeNonAccessible = context.IncludeNonAccessible;

            projectDirectory = context.ProjectDirectory;

            //modifiersCompiler = new(context);

            nodeDebugger = context.Debuggers.NodeDebugger;
        }

        protected bool IsIncluded(SymbolModifiers modifiers)
            => includeNonAccessible || modifiers.IsAccessible;
    }
}
