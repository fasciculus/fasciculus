using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.CodeAnalysis.Models;
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

        public PackageSymbol Compile()
        {
            ParsedProject project = context.Project;

            IEnumerable<CompilationUnitSyntax> roots = project
                .Where(t => t.HasCompilationUnitRoot)
                .Select(t => t.GetCompilationUnitRoot());

            SymbolName name = new(project.AssemblyName);
            UriPath link = new(name);
            TargetFramework framework = context.Framework;
            CompilationUnitCompiler compiler = new(context);
            FileInfo commentFile = context.CommentsDirectory.File(context.Project.AssemblyName + ".xml");
            SymbolComment comment = SymbolComment.FromFile(commentFile);
            UriPath repositoryDirectory = context.Project.RepositoryDirectory;
            CompilationUnitInfo[] compilationUnits = [.. roots.Select(compiler.Compile)];

            return new(name, framework, compilationUnits)
            {
                Name = name,
                Link = link,
                Comment = comment,
                RepositoryDirectory = repositoryDirectory
            };
        }
    }
}
