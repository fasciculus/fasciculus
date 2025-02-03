using Fasciculus.Html;
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
            handled.UnionWith(HtmlConstants.TagNames);
        }

        public void Handled(IEnumerable<string> tagNames)
        {
            handled.UnionWith(tagNames);
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
