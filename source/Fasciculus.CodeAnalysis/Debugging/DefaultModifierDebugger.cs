using Fasciculus.Threading.Synchronization;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Debugging
{
    public class DefaultModifierDebugger : IModifierDebugger
    {
        private readonly TaskSafeMutex mutex = new();

        private readonly SortedSet<string> handled;
        private readonly SortedSet<string> used = [];

        public DefaultModifierDebugger()
        {
            handled = new([
                "public", "private", "protected", "internal",
                "abstract", "static", "sealed", "readonly",
                "virtual", "override", "unsafe", "async",
                "partial", "params", "this", "out"]);
        }

        public void Add(string modifier)
        {
            using Locker locker = Locker.Lock(mutex);

            used.Add(modifier);
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
