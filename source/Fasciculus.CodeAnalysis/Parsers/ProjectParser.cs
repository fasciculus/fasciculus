using Fasciculus.CodeAnalysis.Frameworks;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.CodeAnalysis.Support;
using Fasciculus.Collections;
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
        private readonly Project project;

        /// <summary>
        /// Creates a new parser for the given <paramref name="project"/>.
        /// </summary>
        public ProjectParser(Project project)
        {
            this.project = project;
        }

        /// <summary>
        /// Parses the project.
        /// </summary>
        /// <returns></returns>
        public PackageInfo Parse(bool includeGenerated)
        {
            PackageInfo result = new(project.AssemblyName);

            if (Tasks.Wait(project.GetCompilationAsync()) is CSharpCompilation compilation)
            {
                compilation.SyntaxTrees
                    .Where(t => CheckGenerated(t, includeGenerated))
                    .Where(t => t.HasCompilationUnitRoot)
                    .Select(t => t.GetCompilationUnitRoot())
                    .Select(cu => new CompilationUnitParser(cu))
                    .Select(p => p.Parse())
                    .Apply(result.Namespaces.MergeWith);
            }

            result.Namespaces.AddPackage(result.Name);

            if (project.TryGetTargetFramework(out TargetFramework? framework))
            {
                result.AddFramework(framework);
            }

            return result;
        }

        private static bool CheckGenerated(SyntaxTree syntaxTree, bool includeGenerated)
            => includeGenerated || !syntaxTree.IsGenerated();

        /// <summary>
        /// Parses the given projects.
        /// </summary>
        public static PackageCollection Parse(IEnumerable<Project> projects, bool includeGenerated)
        {
            PackageInfo[] packages = [.. projects
                .Where(p => p.HasDocuments)
                .Select(p => new ProjectParser(p))
                .AsParallel()
                .Select(p => p.Parse(includeGenerated))];

            return new(packages);
        }
    }
}
