using System.Threading;
using System.Threading.Tasks;

namespace Fasciculus.Threading
{
    /// <summary>
    /// Interface implemented by task-safe mutex classes.
    /// </summary>
    public interface ITaskSafeMutex : IUnlockable
    {
        /// <summary>
        /// Locks this synchronization object.
        /// </summary>
        public Task Lock(CancellationToken cancellationToken);

    }
}
