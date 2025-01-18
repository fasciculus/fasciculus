using Fasciculus.CodeAnalysis.Debugging;
using Fasciculus.CodeAnalysis.Extensions;
using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.IO;
using Fasciculus.Net.Navigating;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.IO;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public partial class CompilationUnitCompiler : CSharpSyntaxWalker
    {
        private readonly CompilerContext context;

        public TargetFramework Framework { get; }

        public string Package { get; }

        public DirectoryInfo NamespaceCommentsDirectory { get; }

        public bool IncludeNonAccessible { get; }

        public ModifiersCompiler ModifiersCompiler { get; }

        public INodeDebugger NodeDebugger { get; }

        protected UriPath Source { get; set; } = UriPath.Empty;

        protected CompilationUnitInfo compilationUnit = new();

        public CompilationUnitCompiler(CompilerContext context)
            : base(SyntaxWalkerDepth.StructuredTrivia)
        {
            this.context = context;

            Framework = context.Framework;
            Package = context.Project.AssemblyName;
            NamespaceCommentsDirectory = context.CommentsDirectory.Combine("Namespaces");
            IncludeNonAccessible = context.IncludeNonAccessible;
            ModifiersCompiler = new(context);
            NodeDebugger = context.Debuggers.NodeDebugger;
        }

        public virtual CompilationUnitInfo Compile(CompilationUnitSyntax node)
        {
            Source = node.GetSource(context.ProjectDirectory);

            compilationUnit = new();

            node.Accept(this);

            return compilationUnit;
        }

        protected virtual UriPath GetSource(SyntaxNode node)
        {
            return UriPath.Empty;
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

        protected bool IsIncluded(SymbolModifiers modifiers)
            => IncludeNonAccessible || modifiers.IsAccessible;


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
