using Microsoft.CodeAnalysis.MSBuild;

namespace Fasciculus.CodeAnalysis.Workspaces
{
    /// <summary>
    /// A factory to create workspaces.
    /// </summary>
    public static class WorkspaceFactory
    {
        /// <summary>
        /// Creates a new workspace.
        /// <para>
        /// The workspace's <see cref="MSBuildWorkspace.SkipUnrecognizedProjects" /> property is set to <c>true</c>.
        /// </para>
        /// </summary>
        public static MSBuildWorkspace CreateWorkspace()
        {
            WorkspaceInitializer.Initialize();

            MSBuildWorkspace workspace = MSBuildWorkspace.Create();

            workspace.SkipUnrecognizedProjects = true;

            return workspace;
        }
    }
}
