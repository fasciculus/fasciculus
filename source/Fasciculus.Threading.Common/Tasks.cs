using System.Threading.Tasks;

namespace Fasciculus.Threading
{
    /// <summary>
    /// Utilities for <see cref="Task"/>s.
    /// </summary>
    public static partial class Tasks
    {
        /// <summary>
        /// Synchronously waits for the given <paramref name="task"/> to finish and returns its result.
        /// </summary>
        public static T Wait<T>(Task<T> task)
        {
            Task.WaitAll([task]);

            return task.Result;
        }
    }
}
