using Fasciculus.Threading.Synchronization;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Debugging
{
    public class DefaultCommentDebugger : ICommentDebugger
    {
        private readonly TaskSafeMutex mutex = new();

        private readonly SortedSet<string> handled = [];
        private readonly SortedSet<string> used = [];

        public DefaultCommentDebugger()
        {
            handled.Add("b");
            handled.Add("c");
            handled.Add("code");
            handled.Add("item");
            handled.Add("list");

            handled.Add("para");
            handled.Add("paramref");
            handled.Add("see");
            handled.Add("typeparamref");
        }

        public void Used(string name)
        {
            using Locker locker = Locker.Lock(mutex);

            used.Add(name);
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
