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

        public static IEnumerable<Task<T>> WaitAll<T>(this IEnumerable<Task<T>> tasks)
        {
            Task.WaitAll(tasks.ToArray());

            return tasks;
        }
    }
}
