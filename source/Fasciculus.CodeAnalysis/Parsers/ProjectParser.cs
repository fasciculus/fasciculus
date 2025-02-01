using Fasciculus.CodeAnalysis.Extensions;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Parsers
{
    /// <summary>
    /// Parser for a single project.
    /// </summary>
    public class ProjectParser
    {
        public bool IncludeGenerated { get; }

        public ProjectParser(ProjectParserContext context)
        {
            IncludeGenerated = context.IncludeGenerated;
        }

        public ParsedProject Parse(UnparsedProject unparsedProject)
        {
            Project project = unparsedProject.Project;
            DirectoryInfo projectDirectory = unparsedProject.ProjectDirectory;
            IEnumerable<SyntaxTree> syntaxTrees = [];

            if (Tasks.Wait(project.GetCompilationAsync()) is CSharpCompilation compilation)
            {
                syntaxTrees = compilation.SyntaxTrees.Where(t => CheckGenerated(t, projectDirectory));
            }

            return new(syntaxTrees)
            {
                AssemblyName = project.AssemblyName,
                Framework = project.GetTargetFramework(),
                ProjectDirectory = unparsedProject.ProjectDirectory,
                Repository = unparsedProject.Repository,
            };
        }

        private bool CheckGenerated(SyntaxTree syntaxTree, DirectoryInfo projectDirectory)
            => IncludeGenerated || !syntaxTree.IsGenerated(projectDirectory);
    }
}
