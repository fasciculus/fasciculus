using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Compilers;
using Fasciculus.CodeAnalysis.Indexing;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.CodeAnalysis.Parsers;
using Fasciculus.CodeAnalysis.Workspaces;
using Fasciculus.Collections;
using Microsoft.CodeAnalysis.MSBuild;

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
            ParsedProject[] parsedProjects = ProjectParser2.Parse(workspace.CurrentSolution.Projects, false);
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
            ParsedProject[] parsedProjects = ProjectParser2.Parse(workspace.CurrentSolution.Projects, options.IncludeGenerated);

            return PackageCompiler.Compile(parsedProjects);
        }

        private static SymbolIndices CreateIndices(PackageList packages, PackageSymbol combined)
        {
            return SymbolIndices.Create()
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
