using System.Threading;
using System.Threading.Tasks;

namespace Fasciculus.Threading
{
    /// <summary>
    /// Non-reentrant task-safe mutex.
    /// </summary>
    public class AsyncMutex : IAsyncLockable
    {
        private readonly InterlockedBool locked = new();

        /// <summary>
        /// Locks this mutex.
        /// </summary>
        public async Task LockAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (locked.Replace(true, false))
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
            locked.Write(false);
        }
    }
}
