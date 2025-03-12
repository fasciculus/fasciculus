using System.Threading;
using System.Threading.Tasks;

namespace Fasciculus.Threading.Synchronization
{
    /// <summary>
    /// Non-reentrant task-safe mutex.
    /// </summary>
    public class TaskSafeMutex : ILockable
    {
        private readonly InterlockedBool locked = new();

        /// <summary>
        /// Locks this mutex.
        /// </summary>
        public void Lock(CancellationToken ctk)
        {
            while (true)
            {
                ctk.ThrowIfCancellationRequested();

                if (locked.Replace(true, false))
                {
                    return;
                }

                Task.Yield().GetAwaiter().GetResult();
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
