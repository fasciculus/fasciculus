using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Debugging;
using Fasciculus.CodeAnalysis.Extensions;
using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.CodeAnalysis.Models;
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

        private TargetFramework Framework { get; }
        private string Package { get; }
        private bool IncludeNonAccessible { get; }

        private DirectoryInfo Directory { get; }

        private CommentContext CommentContext { get; }

        private IAccessorDebugger AccessorDebugger { get; }
        private IModifierDebugger ModifierDebugger { get; }
        private INodeDebugger NodeDebugger { get; }

        private UriPath Source { get; set; } = UriPath.Empty;

        private CompilationUnitInfo compilationUnit = new();

        public CompilationUnitCompiler(CompilerContext context)
            : base(SyntaxWalkerDepth.StructuredTrivia)
        {
            Framework = context.Framework;
            Package = context.Project.Name;
            IncludeNonAccessible = context.IncludeNonAccessible;

            Directory = context.Directory;

            CommentContext = context.CommentContext;

            AccessorDebugger = context.Debuggers.AccessorDebugger;
            ModifierDebugger = context.Debuggers.ModifierDebugger;
            NodeDebugger = context.Debuggers.NodeDebugger;
        }

        public virtual CompilationUnitInfo Compile(CompilationUnitSyntax node)
        {
            using Locker locker = Locker.Lock(mutex);

            Source = node.GetSource(Directory);

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

            NodeDebugger.Add(node);

            base.VisitCompilationUnit(node);
        }
    }
}
