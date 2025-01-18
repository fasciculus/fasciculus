using Fasciculus.CodeAnalysis.Extensions;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
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
            IEnumerable<SyntaxTree> syntaxTrees = [];
            Project project = unparsedProject.Project;

            if (Tasks.Wait(project.GetCompilationAsync()) is CSharpCompilation compilation)
            {
                syntaxTrees = compilation.SyntaxTrees.Where(CheckGenerated);
            }

            return new(syntaxTrees)
            {
                AssemblyName = project.AssemblyName,
                Framework = project.GetTargetFramework(),
                ProjectDirectory = project.GetDirectory(),
                RepositoryDirectory = unparsedProject.RepositoryDirectory,
            };
        }

        private bool CheckGenerated(SyntaxTree syntaxTree)
            => IncludeGenerated || !syntaxTree.IsGenerated();
    }
}
