using Fasciculus.ApiDoc.Models;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.ApiDoc
{
    public class ApiParser
    {
        public ApiParser()
        {
        }

        public ApiPackage[] ParseProjects(IEnumerable<Project> projects)
            => [.. projects.Select(ParseProject)];

        public ApiPackage ParseProject(Project project)
        {
            string targetFramework = ParseTargetFramework(project);

            ApiPackage package = new()
            {
                Name = project.AssemblyName
            };

            package.TargetFrameworks.Add(targetFramework);

            return package;
        }

        public static string ParseTargetFramework(Project project)
        {
            string suffix = project.Name[project.AssemblyName.Length..];

            return (suffix.StartsWith('(') && suffix.EndsWith(')')) ? suffix[1..^1] : string.Empty;
        }

        public static ApiPackage[] Parse(IEnumerable<Project> projects)
        {
            ApiParser parser = new ApiParser();

            return parser.ParseProjects(projects);
        }
    }
}
