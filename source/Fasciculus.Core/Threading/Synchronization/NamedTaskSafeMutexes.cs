using System.Collections.Generic;
using System.Threading;

namespace Fasciculus.Threading.Synchronization
{
    public static class NamedTaskSafeMutexes
    {
        private static readonly TaskSafeMutex mutex = new();
        private static readonly Dictionary<string, TaskSafeMutex> mutexes = [];

        public static Locker Lock(string name, CancellationToken ctk = default)
        {
            using Locker locker = Locker.Lock(mutex, ctk);

            mutexes.TryGetValue(name, out TaskSafeMutex? namedMutex);

            if (namedMutex is null)
            {
                mutexes[name] = namedMutex = new();
            }

            return Locker.Lock(namedMutex, ctk);
        }
    }
}
