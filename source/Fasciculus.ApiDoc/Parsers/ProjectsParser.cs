using Fasciculus.ApiDoc.Models;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Fasciculus.ApiDoc.Parsers
{
    public class ProjectsParser
    {
        public static ApiPackages Parse(IEnumerable<Project> projects)
        {
            ApiPackages packages = new();

            foreach (Project project in projects)
            {
                ApiPackage package = packages[project.AssemblyName];
                string targetFramework = ParseTargetFramework(project);

                if (targetFramework.Length > 0)
                {
                    package.TargetFrameworks.Add(targetFramework);
                }
            }

            return packages;
        }

        private static string ParseTargetFramework(Project project)
        {
            string suffix = project.Name.Substring(project.AssemblyName.Length);

            if (suffix.StartsWith('(') && suffix.EndsWith(')'))
            {
                return suffix[1..^1];
            }

            return string.Empty;
        }
    }
}
