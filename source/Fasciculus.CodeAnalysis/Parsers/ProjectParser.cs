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

        public ParsedProject Parse(UnparsedProject unparsed)
        {
            Project project = unparsed.Project;
            DirectoryInfo directory = unparsed.Directory;
            IEnumerable<SyntaxTree> syntaxTrees = [];

            if (Tasks.Wait(project.GetCompilationAsync()) is CSharpCompilation compilation)
            {
                syntaxTrees = compilation.SyntaxTrees.Where(t => CheckGenerated(t, directory));
            }

            return new(syntaxTrees)
            {
                Name = project.AssemblyName,
                Framework = project.GetTargetFramework(),
                Directory = directory,
                Repository = unparsed.Repository,
            };
        }

        private bool CheckGenerated(SyntaxTree syntaxTree, DirectoryInfo directory)
            => IncludeGenerated || !syntaxTree.IsGenerated(directory);
    }
}
