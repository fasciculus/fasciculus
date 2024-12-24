using System.Threading;

namespace Fasciculus.Threading.Synchronization
{
    /// <summary>
    /// Non-reentrant task-safe mutex.
    /// </summary>
    public class TaskSafeMutex : ILockable
    {
        private long locked = 0;

        /// <summary>
        /// Locks this mutex.
        /// </summary>
        public void Lock(CancellationToken ctk)
        {
            while (true)
            {
                ctk.ThrowIfCancellationRequested();

                if (Interlocked.CompareExchange(ref locked, 1, 0) == 0)
                {
                    return;
                }

                Tasks.Yield();
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
