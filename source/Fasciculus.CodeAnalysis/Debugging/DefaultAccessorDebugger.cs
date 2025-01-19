using Fasciculus.Threading.Synchronization;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Debugging
{
    public class DefaultAccessorDebugger : IAccessorDebugger
    {
        private readonly TaskSafeMutex mutex = new();

        private readonly SortedSet<string> handled;
        private readonly SortedSet<string> used = [];

        public DefaultAccessorDebugger()
        {
            handled = new([]);
        }

        public void Add(string accessor)
        {
            using Locker locker = Locker.Lock(mutex);

            used.Add(accessor);
        }

        public SortedSet<string> GetUnhandled()
        {
            using Locker locker = Locker.Lock(mutex);

            SortedSet<string> unhandled = new(used);

            unhandled.ExceptWith(handled);

            return unhandled;
        }
    }
}
