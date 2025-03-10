using System.Threading;
using System.Threading.Tasks;

namespace Fasciculus.Threading
{
    /// <summary>
    /// Non-reentrant task-safe mutex.
    /// </summary>
    public class TaskSafeMutex : ITaskSafeMutex
    {
        private long locked = 0;

        /// <summary>
        /// Locks this mutex.
        /// </summary>
        public async Task Lock(CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (Interlocked.CompareExchange(ref locked, 1, 0) == 0)
                {
                    return;
                }

                await Task.Yield();
            }
        }

        /// <summary>
        /// Unlocks this mutex.
        /// </summary>
        public void Unlock()
        {
            Interlocked.Exchange(ref locked, 0);
        }
    }
}
