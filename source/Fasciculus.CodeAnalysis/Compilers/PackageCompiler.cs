using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class PackageCompiler
    {
        private readonly CompilerContext context;

        public PackageCompiler(CompilerContext context)
        {
            this.context = context;
        }

        public PackageSymbol Compile(ParsedProject project)
        {
            IEnumerable<CompilationUnitSyntax> roots = project
                .Where(t => t.HasCompilationUnitRoot)
                .Select(t => t.GetCompilationUnitRoot());

            SymbolName name = new(project.Name);
            UriPath link = new(name);
            CompilationCompiler compiler = new(context.WithFramework(project.Framework));

            IEnumerable<CompilationUnit> compilationUnits = roots
                .Select(root => compiler.Compile(root, link));

            return new(name, link, context.Frameworks, compilationUnits);
        }
    }
}
