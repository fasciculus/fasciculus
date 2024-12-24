using System.Threading;

namespace Fasciculus.Threading.Synchronization
{
    /// <summary>
    /// Interface implemented by synchronization classes.
    /// <para>
    /// Note: Calls to <see cref="Lock(CancellationToken)"/> and <see cref="Unlock"/> must always be done in pairs.
    /// </para>
    /// </summary>
    public interface ILockable
    {
        /// <summary>
        /// Locks this synchronization object.
        /// </summary>
        /// <param name="ctk">Token to abort locking.</param>
        public void Lock(CancellationToken ctk);

        /// <summary>
        /// Unlocks this synchronization object.
        /// </summary>
        public void Unlock();
    }
}
