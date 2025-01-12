using Fasciculus.Threading.Synchronization;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Support
{
    public class SymbolCounters
    {
        public static readonly SymbolCounters Instance = new();

        private readonly TaskSafeMutex mutex = new();
        private readonly Dictionary<string, ulong> counters = [];

        public Dictionary<string, ulong> Counters
            => Locker.Locked(mutex, () => new Dictionary<string, ulong>(counters));

        private SymbolCounters() { }

        public void Increment(string name)
        {
            using Locker locker = Locker.Lock(mutex);

            if (!counters.TryGetValue(name, out ulong value))
            {
                value = 0;
                counters[name] = value;
            }

            counters[name] = ++value;
        }
    }
}
