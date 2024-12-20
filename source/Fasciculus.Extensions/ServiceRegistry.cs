using Fasciculus.Support;
using Fasciculus.Threading;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceRegistry
    {
        private static readonly TaskSafeMutex mutex = new();

        private static IServiceProvider? services = null;

        public static void Initialize(IServiceProvider services)
        {
            using Locker locker = Locker.Lock(mutex);

            Cond.IsNull(ServiceRegistry.services);

            ServiceRegistry.services = services;
        }

        public static T GetRequiredService<T>()
            where T : notnull
        {
            using Locker locker = Locker.Lock(mutex);

            return Cond.NotNull(services).GetRequiredService<T>();
        }
    }
}
