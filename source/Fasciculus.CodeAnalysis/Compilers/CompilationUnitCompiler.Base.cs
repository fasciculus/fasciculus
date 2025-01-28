using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Debugging;
using Fasciculus.CodeAnalysis.Extensions;
using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.IO;
using Fasciculus.Net.Navigating;
using Fasciculus.Threading.Synchronization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.IO;

namespace Fasciculus.CodeAnalysis.Compilers
{
    internal partial class CompilationUnitCompiler : CSharpSyntaxWalker
    {
        private readonly TaskSafeMutex mutex = new();

        private readonly TargetFramework framework;
        private readonly string package;
        protected readonly bool includeNonAccessible;

        private readonly DirectoryInfo projectDirectory;
        private readonly DirectoryInfo namespaceCommentsDirectory;

        private readonly CommentContext commentContext;

        private readonly IAccessorDebugger accessorDebugger;
        private readonly IModifierDebugger modifierDebugger;
        private readonly INodeDebugger nodeDebugger;

        private UriPath Source { get; set; } = UriPath.Empty;

        private CompilationUnitInfo compilationUnit = new();

        public CompilationUnitCompiler(CompilerContext context)
            : base(SyntaxWalkerDepth.StructuredTrivia)
        {
            framework = context.Framework;
            package = context.Project.AssemblyName;
            includeNonAccessible = context.IncludeNonAccessible;

            projectDirectory = context.ProjectDirectory;
            namespaceCommentsDirectory = context.CommentsDirectory.Combine("Namespaces");

            commentContext = context.CommentContext;

            accessorDebugger = context.Debuggers.AccessorDebugger;
            modifierDebugger = context.Debuggers.ModifierDebugger;
            nodeDebugger = context.Debuggers.NodeDebugger;
        }

        public virtual CompilationUnitInfo Compile(CompilationUnitSyntax node)
        {
            using Locker locker = Locker.Lock(mutex);

            Source = node.GetSource(projectDirectory);

            compilationUnit = new();

            node.Accept(this);

            return compilationUnit;
        }

        public override void VisitCompilationUnit(CompilationUnitSyntax node)
        {
            // compilation_unit
            //   : extern_alias_directive* using_directive* global_attributes? namespace_member_declaration*
            //
            // CompilationUnit: UsingDirective* AttributeList* NamespaceDeclaration*

            nodeDebugger.Add(node);

            base.VisitCompilationUnit(node);
        }
    }
}
