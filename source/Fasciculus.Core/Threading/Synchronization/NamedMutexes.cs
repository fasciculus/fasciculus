using System;
using System.Collections.Generic;
using System.Threading;

namespace Fasciculus.Threading.Synchronization
{
    /// <summary>
    /// Manager for reentrant named task-safe mutexes.
    /// </summary>
    public static class NamedMutexes
    {
        private static readonly TaskSafeMutex mutex = new();
        private static readonly Dictionary<string, ReentrantTaskSafeMutex> mutexes = [];

        /// <summary>
        /// Locks a task-safe mutex with the given name, creating it if required.
        /// </summary>
        public static Locker Lock(string name, CancellationToken ctk = default)
            => Locker.Lock(GetMutex(name, ctk), ctk);


        /// <summary>
        /// Executes the given <paramref name="action"/> within named lock.
        /// </summary>
        public static void Locked(string name, Action action, CancellationToken ctk = default)
            => Locker.Locked(GetMutex(name, ctk), action, ctk);

        /// <summary>
        /// Executes the given <paramref name="func"/> within named lock.
        /// </summary>
        public static T Locked<T>(string name, Func<T> func, CancellationToken ctk = default)
            => Locker.Locked(GetMutex(name, ctk), func, ctk);

        /// <summary>
        /// Returns a reentrant mutex for the given name.
        /// </summary>
        public static ReentrantTaskSafeMutex GetMutex(string name, CancellationToken ctk)
        {
            using Locker locker = Locker.Lock(mutex, ctk);

            mutexes.TryGetValue(name, out ReentrantTaskSafeMutex? namedMutex);

            if (namedMutex is null)
            {
                namedMutex = new();
                mutexes.Add(name, namedMutex);
            }

            return namedMutex;
        }
    }
}
