using Fasciculus.Collections;
using Fasciculus.Threading.Synchronization;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class UnhandledModifiers
    {
        public static readonly UnhandledModifiers Instance = new();

        private readonly TaskSafeMutex mutex = new();

        private SortedSet<string> handled = [];
        private SortedSet<string> used = [];

        private UnhandledModifiers() { }

        public void Handled(IEnumerable<string> modifiers)
            => Locker.Locked(mutex, () => { modifiers.Apply(h => { handled.Add(h); }); });

        public void Used(string modifier)
            => Locker.Locked(mutex, () => { used.Add(modifier); });

        public SortedSet<string> Unhandled()
        {
            using Locker locker = Locker.Lock(mutex);

            SortedSet<string> unhandled = new(used);

            unhandled.ExceptWith(handled);

            return unhandled;
        }
    }
}
