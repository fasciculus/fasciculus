using Fasciculus.CodeAnalysis.Debugging;
using Fasciculus.CodeAnalysis.Extensions;
using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.IO;
using Fasciculus.Net.Navigating;
using Fasciculus.Threading.Synchronization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.IO;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public partial class CompilationUnitCompiler : CompilerBase
    {
        private readonly TaskSafeMutex mutex = new();

        public TargetFramework Framework { get; }

        public string Package { get; }

        public DirectoryInfo ProjectDirectory { get; }

        public DirectoryInfo NamespaceCommentsDirectory { get; }

        public ModifiersCompiler ModifiersCompiler { get; }

        private readonly AccessorsCompiler accessorsCompiler;

        public INodeDebugger NodeDebugger { get; }

        protected UriPath Source { get; set; } = UriPath.Empty;

        protected CompilationUnitInfo compilationUnit = new();

        public CompilationUnitCompiler(CompilerContext context)
            : base(context, SyntaxWalkerDepth.StructuredTrivia)
        {
            Framework = context.Framework;
            Package = context.Project.AssemblyName;
            ProjectDirectory = context.ProjectDirectory;
            NamespaceCommentsDirectory = context.CommentsDirectory.Combine("Namespaces");
            ModifiersCompiler = new(context);
            accessorsCompiler = new(context);
            NodeDebugger = context.Debuggers.NodeDebugger;
        }

        public virtual CompilationUnitInfo Compile(CompilationUnitSyntax node)
        {
            using Locker locker = Locker.Lock(mutex);

            Source = node.GetSource(ProjectDirectory);

            compilationUnit = new();

            node.Accept(this);

            return compilationUnit;
        }

        protected virtual SymbolName GetName(SyntaxToken identifier, TypeParameterListSyntax? typeParameters)
        {
            string name = identifier.Text;

            if (typeParameters is null || typeParameters.Parameters.Count == 0)
            {
                return new(name);
            }

            string[] parameters = [.. typeParameters.Parameters.Select(p => p.Identifier.Text)];

            return new(name, parameters);
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

        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
            // UsingDirective
            // : IdentifierName
            // | QualifiedName

            NodeDebugger.Add(node);

            base.VisitUsingDirective(node);
        }

    }
}
