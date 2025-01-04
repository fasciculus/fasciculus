using Fasciculus.Threading;
using Fasciculus.Threading.Synchronization;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Fasciculus.CodeAnalysis.Workspaces
{
    /// <summary>
    /// Extensions for <see cref="MSBuildWorkspace"/>
    /// </summary>
    public static class MSBuildWorkspaceExtensions
    {
        private static readonly TaskSafeMutex mutex = new();

        /// <summary>
        /// Adds the given <paramref name="projectFile"/> to the given <paramref name="workspace"/> if it hasn't added yet.
        /// </summary>
        public static async Task<MSBuildWorkspace> AddProjectFileAsync(this MSBuildWorkspace workspace, FileInfo projectFile,
            IProgress<ProjectLoadProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            using Locker locker = Locker.Lock(mutex, cancellationToken);

            if (!workspace.HasProjectFile(projectFile))
            {
                await workspace.OpenProjectAsync(projectFile.FullName, progress, cancellationToken);
            }

            return workspace;
        }

        /// <summary>
        /// Adds the given <paramref name="projectFile"/> to the given <paramref name="workspace"/> if it hasn't added yet.
        /// </summary>
        public static MSBuildWorkspace AddProjectFile(this MSBuildWorkspace workspace, FileInfo projectFile,
            IProgress<ProjectLoadProgress>? progress = null)
        {
            return Tasks.Wait(workspace.AddProjectFileAsync(projectFile, progress));
        }
    }
}
