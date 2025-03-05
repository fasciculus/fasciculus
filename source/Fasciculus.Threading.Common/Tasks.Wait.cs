using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fasciculus.Threading
{
    public static partial class Tasks
    {
        /// <summary>
        /// Synchronously waits for the given <paramref name="tasks"/> to finish.
        /// </summary>
        public static void Wait(Task[] tasks)
            => Task.WaitAll(tasks);

        /// <summary>
        /// Synchronously waits for the given <paramref name="tasks"/> to finish.
        /// </summary>
        public static void Wait(IEnumerable<Task> tasks)
            => Wait([.. tasks]);

        /// <summary>
        /// Synchronously waits for the given <paramref name="tasks"/> to finish and returns their results.
        /// </summary>
        public static T[] Wait<T>(Task<T>[] tasks)
        {
            Task.WaitAll(tasks);

            return [.. tasks.Select(task => task.Result)];
        }

        /// <summary>
        /// Synchronously waits for the given <paramref name="tasks"/> to finish and returns their results.
        /// </summary>
        public static T[] Wait<T>(IEnumerable<Task<T>> tasks)
            => Wait([.. tasks]);

        /// <summary>
        /// Synchronously waits for the given <paramref name="task"/> to finish.
        /// </summary>
        public static void Wait(Task task)
            => Wait([task]);

        /// <summary>
        /// Synchronously waits for the given <paramref name="task"/> to finish and returns its result.
        /// </summary>
        public static T Wait<T>(Task<T> task)
            => Wait([task])[0];
    }
}