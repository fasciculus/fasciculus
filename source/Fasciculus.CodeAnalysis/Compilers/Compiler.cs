using Fasciculus.CodeAnalysis.Debugging;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public partial class Compiler : CSharpSyntaxWalker
    {
        private readonly CompilerContext context;

        protected readonly ModifierCompiler modifierCompiler;

        protected readonly INodeDebugger nodeDebugger;

        public Compiler(CompilerContext context)
        {
            this.context = context;

            modifierCompiler = new(context);

            nodeDebugger = context.Debuggers.NodeDebugger;
        }

        public virtual void Compile(CompilationUnitSyntax compilationUnit)
        {
            compilationUnit.Accept(this);
        }

        protected virtual string GetName(SyntaxToken identifier, TypeParameterListSyntax? typeParameters)
        {
            string name = identifier.Text;

            if (typeParameters is null || typeParameters.Parameters.Count == 0)
            {
                return name;
            }

            string parameters = string.Join(",", typeParameters.Parameters.Select(p => p.Identifier.Text));

            return $"{name}<{parameters}>";
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
