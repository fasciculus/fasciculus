using Fasciculus.CodeAnalysis.Models;
using Fasciculus.CodeAnalysis.Support;
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
    public class ProjectParser2
    {
        public ParsedProject ParseProject(Project project, bool includeGenerated)
        {
            IEnumerable<SyntaxTree> syntaxTrees = [];

            if (Tasks.Wait(project.GetCompilationAsync()) is CSharpCompilation compilation)
            {
                syntaxTrees = compilation.SyntaxTrees.Where(t => CheckGenerated(t, includeGenerated));
            }

            return new(project.AssemblyName, syntaxTrees);
        }

        private static bool CheckGenerated(SyntaxTree syntaxTree, bool includeGenerated)
            => includeGenerated || !syntaxTree.IsGenerated();


        public static ParsedProject Parse(Project project, bool includeGenerated)
        {
            if (project.HasDocuments)
            {
                ProjectParser2 parser = new();

                return parser.ParseProject(project, includeGenerated);
            }

            return new(project.AssemblyName);
        }

        public static ParsedProject[] Parse(IEnumerable<Project> projects, bool includeGenerated)
        {
            Project[] projectsWithDocuments = [.. projects.Where(p => p.HasDocuments)];

            if (projectsWithDocuments.Length == 0)
            {
                return [];
            }

            ProjectParser2 parser = new();

            return [.. projectsWithDocuments.AsParallel().Select(p => parser.ParseProject(p, includeGenerated))];
        }
    }
}
