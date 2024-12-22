using Fasciculus.Support;
using Fasciculus.Threading.Synchronization;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Global service provider.
    /// </summary>
    public static class ServiceRegistry
    {
        private static readonly TaskSafeMutex mutex = new();

        private static IServiceProvider? services = null;

        /// <summary>
        /// Initializes the global service provider.
        /// <para>Call after <c>MauiAppBuilder.Build()</c></para>
        /// </summary>
        public static void Initialize(IServiceProvider services)
        {
            using Locker locker = Locker.Lock(mutex);

            Cond.IsNull(ServiceRegistry.services);

            ServiceRegistry.services = services;
        }

        /// <summary>
        /// Get service of type <typeparamref name="T"/> from the <see cref="IServiceProvider"/>.
        /// </summary>
        /// <typeparam name="T">The type of service object to get.</typeparam>
        /// <returns>A service object of type <typeparamref name="T"/>.</returns>
        /// <exception cref="InvalidOperationException">There is no service of type <typeparamref name="T"/>.</exception>
        public static T GetRequiredService<T>()
            where T : notnull
        {
            using Locker locker = Locker.Lock(mutex);

            return Cond.NotNull(services).GetRequiredService<T>();
        }
    }
}
