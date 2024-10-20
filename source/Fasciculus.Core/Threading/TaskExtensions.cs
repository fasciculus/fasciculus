using System.Collections.Generic;
using System.Linq;

namespace System.Threading.Tasks
{
    public static class TaskExtensions
    {
        public static async Task WaitAll(this IEnumerable<Task> tasks)
        {
            Task.WaitAll(tasks.ToArray());
            await Task.CompletedTask;
        }
    }
}
