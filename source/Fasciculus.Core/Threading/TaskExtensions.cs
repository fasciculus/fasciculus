using System.Collections.Generic;
using System.Linq;

namespace System.Threading.Tasks
{
    public static class TaskExtensions
    {
        public static void WaitAll(this IEnumerable<Task> tasks)
        {
            Task.WaitAll(tasks.ToArray());
        }
    }
}
