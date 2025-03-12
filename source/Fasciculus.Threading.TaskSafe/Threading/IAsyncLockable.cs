using System.Threading;
using System.Threading.Tasks;

namespace Fasciculus.Threading
{
    /// <summary>
    /// Interface implemented by classes lockable by <see cref="AsyncLock"/>.
    /// </summary>
    public interface IAsyncLockable
    {
        /// <summary>
        /// Locks this synchronization object.
        /// </summary>
        public Task LockAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Unlocks this synchronization object.
        /// </summary>
        public void Unlock();
    }
}
