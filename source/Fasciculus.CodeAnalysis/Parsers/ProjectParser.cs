using Fasciculus.CodeAnalysis.Frameworks;
using Fasciculus.CodeAnalysis.Models;
using Microsoft.CodeAnalysis;
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
        public PackageInfo Parse()
        {
            PackageInfo result = new()
            {
                Name = project.AssemblyName
            };

            if (project.TryGetTargetFramework(out TargetFramework? framework))
            {
                result.Frameworks.Add(framework);
            }

            return result;
        }

        /// <summary>
        /// Parses the given projects.
        /// </summary>
        public static Packages Parse(IEnumerable<Project> projects)
        {
            PackageInfo[] packages = [.. projects
                .Where(p => p.HasDocuments)
                .Select(p => new ProjectParser(p))
                .AsParallel()
                .Select(p => p.Parse())];

            return new(packages);
        }
    }
}
