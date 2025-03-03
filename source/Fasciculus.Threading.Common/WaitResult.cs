using System.Threading.Tasks;

namespace Fasciculus.Threading
{
    /// <summary>
    /// Extension methods for <see cref="Task"/>.
    /// </summary>
    public static partial class TaskExtensions
    {
        /// <summary>
        /// Waits for the <see cref="Task"/> to finish.
        /// </summary>
        public static void WaitResult(this Task task)
            => task.GetAwaiter().GetResult();

        /// <summary>
        /// Awaits a <see cref="Task"/> and returns the result.
        /// </summary>
        public static T WaitResult<T>(this Task<T> task)
            => task.GetAwaiter().GetResult();
    }
}