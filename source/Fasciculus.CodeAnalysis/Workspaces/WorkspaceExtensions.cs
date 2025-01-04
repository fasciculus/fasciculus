using Microsoft.CodeAnalysis;
using System.IO;

namespace Fasciculus.CodeAnalysis.Workspaces
{
    /// <summary>
    /// Extensions for <see cref="Workspace"/>.
    /// </summary>
    public static class WorkspaceExtensions
    {
        /// <summary>
        /// Returns whether the given <paramref name="projectFile"/> is already loaded into the given <paramref name="workspace"/>.
        /// </summary>
        public static bool HasProjectFile(this Workspace workspace, FileInfo projectFile)
            => workspace.CurrentSolution.HasProjectFile(projectFile);
    }
}
