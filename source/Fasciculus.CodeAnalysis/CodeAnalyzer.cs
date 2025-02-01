using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Compilers;
using Fasciculus.CodeAnalysis.Configuration;
using Fasciculus.CodeAnalysis.Extensions;
using Fasciculus.CodeAnalysis.Indexing;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.CodeAnalysis.Parsers;
using Fasciculus.CodeAnalysis.Workspaces;
using Fasciculus.Collections;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.CodeAnalysis
{
    public class CodeAnalyzer
    {
        private readonly CodeAnalyzerOptions options;

        private readonly CommentContext commentContext;

        public CodeAnalyzer(CodeAnalyzerOptions options)
        {
            this.options = options;

            commentContext = new()
            {
                Merger = new DefaultCommentMerger(),
                Resolver = new DefaultCommentResolver(options.Debuggers.CommentDebugger),
                Formatter = new DefaultCommentFormatter(),
            };
        }

        public ICodeAnalyzerResult Analyze()
        {
            using MSBuildWorkspace workspace = LoadWorkspace();
            PackageList packages = CompilePackages(workspace);
            PackageSymbol combined = packages.Combine(options.CombinedPackageName, options.CombinedPackageLink, commentContext);
            SymbolIndex index = CreateIndex(packages, combined);

            return new CodeAnalyzerResult(packages, combined, index);
        }

        private PackageList CompilePackages(MSBuildWorkspace workspace)
        {
            ParsedProject[] parsedProjects = [.. ParseProjects(workspace)];
            PackageSymbol[] packages = [.. parsedProjects.Select(CompilePackage)];

            return new(packages);
        }

        private PackageSymbol CompilePackage(ParsedProject project)
        {
            CompilerContext context = new()
            {
                Project = project,
                IncludeNonAccessible = options.IncludeNonAccessible,
                CommentContext = commentContext,
                Debuggers = options.Debuggers,
            };

            PackageCompiler compiler = new(context);

            return compiler.Compile();
        }

        private IEnumerable<ParsedProject> ParseProjects(MSBuildWorkspace workspace)
        {
            Project[] projects = [.. workspace.CurrentSolution.Projects];

            foreach (Project project in projects)
            {
                if (options.Projects.Any(p => p.File.FullName == project.FilePath))
                {
                    if (project.HasDocuments)
                    {
                        DirectoryInfo? projectDirectory = project.GetDirectory();

                        if (projectDirectory is not null)
                        {
                            yield return ParseProject(project, projectDirectory);
                        }
                    }
                }
            }
        }

        private ParsedProject ParseProject(Project project, DirectoryInfo projectDirectory)
        {
            ProjectParserContext context = new()
            {
                IncludeGenerated = options.IncludeGenerated
            };

            ProjectParser parser = new(context);

            UnparsedProject unparsedProject = new()
            {
                Project = project,
                ProjectDirectory = projectDirectory,
                Repository = GetRepository(project),
                Framework = project.GetTargetFramework()
            };

            return parser.Parse(unparsedProject);
        }

        private Uri GetRepository(Project project)
            => options.Projects.First(p => p.File.FullName == project.FilePath).Repository;

        private SymbolIndex CreateIndex(PackageList packages, PackageSymbol combined)
        {
            SymbolIndexOptions options = new()
            {
                IncludeNonAccessible = this.options.IncludeNonAccessible
            };

            return SymbolIndex.Create(options)
                .WithPackages(packages)
                .WithPackages(combined)
                .Build();
        }

        private MSBuildWorkspace LoadWorkspace()
        {
            MSBuildWorkspace workspace = WorkspaceFactory.CreateWorkspace();

            options.Projects.Apply(f => workspace.AddProjectFile(f.File));

            return workspace;
        }

        public static CodeAnalyzerBuilder Create()
            => new();
    }
}
