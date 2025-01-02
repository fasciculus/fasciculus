using Fasciculus.Threading.Synchronization;
using Microsoft.Build.Locator;
using System.Linq;

namespace Fasciculus.ApiDoc.Support
{
    public static class MSBuildInitializer
    {
        private static TaskSafeMutex mutex = new();

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
