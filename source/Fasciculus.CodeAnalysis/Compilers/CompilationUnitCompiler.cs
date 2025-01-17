using Fasciculus.CodeAnalysis.Debugging;
using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.CodeAnalysis.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public partial class CompilationUnitCompiler : CSharpSyntaxWalker
    {
        private readonly CompilerContext context;

        public TargetFramework Framework => context.Framework;

        public string Package => context.Project.AssemblyName;

        protected readonly ModifiersCompiler modifierCompiler;

        protected readonly INodeDebugger nodeDebugger;

        protected CompilationUnitInfo compilationUnit = new();

        public CompilationUnitCompiler(CompilerContext context)
            : base(SyntaxWalkerDepth.StructuredTrivia)
        {
            this.context = context;

            modifierCompiler = new(context);

            nodeDebugger = context.Debuggers.NodeDebugger;
        }

        public virtual CompilationUnitInfo Compile(CompilationUnitSyntax node)
        {
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

        protected bool IsIncluded(SymbolModifiers modifiers)
            => modifiers.IsAccessible || context.IncludeNonAccessible;


        public override void VisitCompilationUnit(CompilationUnitSyntax node)
        {
            // compilation_unit
            //   : extern_alias_directive* using_directive* global_attributes? namespace_member_declaration*
            //
            // CompilationUnit: UsingDirective* AttributeList* NamespaceDeclaration*

            nodeDebugger.Add(node);

            base.VisitCompilationUnit(node);
        }

        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
            // UsingDirective
            // : IdentifierName
            // | QualifiedName

            nodeDebugger.Add(node);

            base.VisitUsingDirective(node);
        }

    }
}
