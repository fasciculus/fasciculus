using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net;
using Fasciculus.Support;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class PackageCompiler
    {
        private readonly CompilerContext context;

        private readonly CompilationCompiler compilationCompiler;

        public PackageCompiler(CompilerContext context)
        {
            this.context = context;

            compilationCompiler = new(context);
        }

        public PackageSymbol Compile(ParsedProject project)
        {
            if (!context.Framework.Equals(project.Framework))
            {
                throw Ex.InvalidOperation();
            }

            IEnumerable<CompilationUnitSyntax> roots = project
                .Where(t => t.HasCompilationUnitRoot)
                .Select(t => t.GetCompilationUnitRoot());

            SymbolName name = new(project.Name);
            UriPath link = new(name.Name);

            IEnumerable<CompilationUnit> compilationUnits = roots
                .Select(root => compilationCompiler.Compile(root, link));

            return new(name, link, context.Frameworks, compilationUnits);
        }
    }
}
