using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Fasciculus.Support
{
    public static class GlobalServices
    {
        private static readonly TaskSafeMutex mutex = new TaskSafeMutex();

        private static IServiceProvider? services = null;

        public static void Initialize(IServiceProvider services)
        {
            using Locker locker = Locker.Lock(mutex);

            Cond.IsNull(GlobalServices.services);

            GlobalServices.services = services;
        }

        public static T GetRequiredService<T>()
            where T : notnull
        {
            using Locker locker = Locker.Lock(mutex);

            return Cond.NotNull(services).GetRequiredService<T>();
        }
    }
}
