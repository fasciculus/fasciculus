using Fasciculus.CodeAnalysis.Models;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class PackageCompiler
    {
        public PackageSymbol CompileProject(ParsedProject project)
        {
            IEnumerable<CompilationUnitSyntax> compilationUnits = project
                .Where(t => t.HasCompilationUnitRoot)
                .Select(t => t.GetCompilationUnitRoot());

            return new(new(project.Name), compilationUnits.Select(CompilationUnitCompiler.Compile));
        }

        public static PackageSymbol Compile(ParsedProject project)
        {
            PackageCompiler compiler = new();

            return compiler.CompileProject(project);
        }

        public static PackageList Compile(IEnumerable<ParsedProject> projects)
        {
            PackageCompiler compiler = new();

            return new(projects.AsParallel().Select(compiler.CompileProject));
        }
    }
}
