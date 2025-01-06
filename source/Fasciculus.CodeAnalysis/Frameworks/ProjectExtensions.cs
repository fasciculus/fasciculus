using Microsoft.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace Fasciculus.CodeAnalysis.Frameworks
{
    /// <summary>
    /// Target framework related extensions for
    /// </summary>
    public static class ProjectExtensions
    {
        /// <summary>
        /// Tries to parse the target framework of the given <paramref name="project"/>.
        /// </summary>
        public static bool TryGetTargetFramework(this Project project, [NotNullWhen(true)] out TargetFramework? framework)
        {
            framework = TargetFramework.UnsupportedFramework;

            string suffix = project.Name[project.AssemblyName.Length..];

            if (string.IsNullOrEmpty(suffix))
            {
                return false;
            }

            if (!suffix.StartsWith('(') || !suffix.EndsWith(')'))
            {
                return false;
            }

            framework = TargetFramework.Parse(suffix[1..^1]);

            return true;
        }

        public static TargetFramework GetTargetFramework(this Project project)
            => TryGetTargetFramework(project, out TargetFramework? framework) ? framework : TargetFramework.UnsupportedFramework;
    }
}
