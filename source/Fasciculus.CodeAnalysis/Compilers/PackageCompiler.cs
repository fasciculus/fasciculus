using Fasciculus.CodeAnalysis.Frameworking;
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
        public PackageSymbol CompileProject(ParsedProject project)
        {
            IEnumerable<CompilationUnitSyntax> roots = project
                .Where(t => t.HasCompilationUnitRoot)
                .Select(t => t.GetCompilationUnitRoot());

            SymbolName name = new(project.Name);
            UriPath link = new(name.Name);
            TargetFramework framework = project.Framework;
            TargetFrameworks frameworks = new(framework);
            CompilationCompiler compiler = new(framework);

            IEnumerable<CompilationUnit> compilationUnits = roots
                .Select(root => compiler.Compile(root, link));

            return new(name, link, frameworks, compilationUnits);
        }

        public static PackageSymbol Compile(ParsedProject project)
        {
            PackageCompiler compiler = new();

            return compiler.CompileProject(project);
        }

        public static PackageList Compile(IEnumerable<ParsedProject> projects)
        {
            return new(projects/*.AsParallel()*/.Select(Compile));
        }
    }
}
