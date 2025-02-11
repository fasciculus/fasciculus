using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Compilers
{
    internal class PackageCompiler
    {
        private readonly CompilerContext context;

        public PackageCompiler(CompilerContext context)
        {
            this.context = context;
        }

        public PackageSymbol Compile()
        {
            ParsedProject project = context.Project;

            IEnumerable<CompilationUnitSyntax> roots = project
                .Where(t => t.HasCompilationUnitRoot)
                .Select(t => t.GetCompilationUnitRoot());

            SymbolName name = new(project.Name);
            UriPath link = new(name);
            TargetFramework framework = context.Framework;
            SymbolComment comment = SymbolComment.Empty(context.CommentContext);
            CompilationUnitCompiler compiler = new(context);
            CompilationUnitInfo[] compilationUnits = [.. roots.Select(compiler.Compile)];

            return new(name, framework, comment, compilationUnits)
            {
                Name = name,
                Link = link,
                Modifiers = PackageSymbol.DefaultModifiers(),
                Repository = context.Project.Repository
            };
        }
    }
}
