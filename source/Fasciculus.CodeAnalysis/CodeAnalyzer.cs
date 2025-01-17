using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Compilers;
using Fasciculus.CodeAnalysis.Extensions;
using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.CodeAnalysis.Indexing;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.CodeAnalysis.Parsers;
using Fasciculus.CodeAnalysis.Workspaces;
using Fasciculus.Collections;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis
{
    public class CodeAnalyzer
    {
        private readonly CodeAnalyzerOptions options;

        public CodeAnalyzer(CodeAnalyzerOptions options)
        {
            this.options = options;
        }

        public CodeAnalyzerResult Analyze()
        {
            using MSBuildWorkspace workspace = LoadWorkspace();
            PackageList packages = CompilePackages(workspace);
            PackageSymbol combined = packages.Combine(options.CombinedPackageName);
            SymbolIndices indices = CreateIndices(packages, combined);

            ProcessComments(indices);

            return new()
            {
                Packages = packages,
                Combined = combined,
                Indices = indices
            };
        }

        private PackageList CompilePackages(MSBuildWorkspace workspace)
        {
            ParsedProject[] parsedProjects = ParseProjects(workspace);
            IEnumerable<PackageSymbol> packages = parsedProjects.Select(CompilePackage);

            return new(packages);
        }

        private PackageSymbol CompilePackage(ParsedProject project)
        {
            CompilerContext context = new(project, options.IncludeNonAccessible, options.Debuggers);
            PackageCompiler compiler = new(context);

            return compiler.Compile(project);
        }

        private ParsedProject[] ParseProjects(MSBuildWorkspace workspace)
        {
            return [.. workspace.CurrentSolution.Projects
                .Where(p => p.HasDocuments)
                //.AsParallel()
                .Select(ParseProject)];
        }

        private ParsedProject ParseProject(Project project)
        {
            ProjectParser parser = new();
            TargetFramework framework = project.GetTargetFramework();
            bool includeGenerated = options.IncludeGenerated;

            return parser.Parse(project, framework, includeGenerated);
        }

        private SymbolIndices CreateIndices(PackageList packages, PackageSymbol combined)
        {
            SymbolIndicesOptions options = new()
            {
                IncludeNonAccessible = this.options.IncludeNonAccessible
            };

            return SymbolIndices.Create(options)
                .WithPackages(packages)
                .WithPackages(combined)
                .Build();
        }

        private static void ProcessComments(SymbolIndices indices)
        {
            SymbolCommentProcessor processor = new(indices);

            processor.Process();
        }

        private MSBuildWorkspace LoadWorkspace()
        {
            MSBuildWorkspace workspace = WorkspaceFactory.CreateWorkspace();

            options.ProjectFiles.Apply(f => workspace.AddProjectFile(f));

            return workspace;
        }

        public static CodeAnalyzerBuilder Create()
            => new();
    }
}
