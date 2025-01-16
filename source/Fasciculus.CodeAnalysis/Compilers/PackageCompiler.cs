using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;
using Fasciculus.IO;
using Fasciculus.Net.Navigating;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.IO;
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

            SymbolName name = new(project.AssemblyName);
            UriPath link = new(name);
            CompilationUnitCompiler compiler = new(context);
            FileInfo? commentFile = context.CommentsDirectory?.File(context.Project.AssemblyName + ".xml");
            SymbolComment comment = SymbolComment.FromFile(commentFile);

            roots.Apply(compiler.Compile);

            return new(name, link, context.Frameworks, [])
            {
                Comment = comment,
            };
        }
    }
}
