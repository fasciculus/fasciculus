using Microsoft.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Workspaces
{
    /// <summary>
    /// Extensions for <see cref="Solution"/>.
    /// </summary>
    public static class SolutionExtensions
    {
        /// <summary>
        /// Returns whether the given <paramref name="projectFile"/> is already loaded into the given <paramref name="solution"/>.
        /// </summary>
        public static bool HasProjectFile(this Solution solution, FileInfo projectFile)
            => solution.Projects.Any(project => projectFile.FullName == project.FilePath);
    }
}
