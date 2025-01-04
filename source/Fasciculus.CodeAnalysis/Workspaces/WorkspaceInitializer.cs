using Fasciculus.Threading.Synchronization;
using Microsoft.Build.Locator;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Workspaces
{
    /// <summary>
    /// Initializes the workspace environment.
    /// </summary>
    public static class WorkspaceInitializer
    {
        private static readonly TaskSafeMutex mutex = new();

        /// <summary>
        /// Initializes the workspace environment.
        /// </summary>
        public static void Initialize()
        {
            using Locker locker = Locker.Lock(mutex);

            if (!MSBuildLocator.IsRegistered)
            {
                VisualStudioInstance[] instances = MSBuildLocator.QueryVisualStudioInstances().ToArray();

                if (instances.Length > 0)
                {
                    VisualStudioInstance instance = instances.OrderByDescending(x => x.Version).First();

                    MSBuildLocator.RegisterInstance(instance);
                }
                else
                {
                    MSBuildLocator.RegisterDefaults();
                }
            }
        }
    }
}
