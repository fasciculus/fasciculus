using Fasciculus.CodeAnalysis.Frameworks;
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
            IEnumerable<CompilationUnitSyntax> roots = project
                .Where(t => t.HasCompilationUnitRoot)
                .Select(t => t.GetCompilationUnitRoot());

            SymbolName name = new(project.Name);
            TargetFramework framework = project.Framework;
            CompilationCompiler compiler = new(framework);
            IEnumerable<CompilationUnit> compilationUnits = roots.Select(compiler.Compile);

            return new(name, framework, compilationUnits);
        }

        public static PackageSymbol Compile(ParsedProject project)
        {
            PackageCompiler compiler = new();

            return compiler.CompileProject(project);
        }

        public static PackageList Compile(IEnumerable<ParsedProject> projects)
        {
            PackageCompiler compiler = new();

            return new(projects/*.AsParallel()*/.Select(compiler.CompileProject));
        }
    }
}
