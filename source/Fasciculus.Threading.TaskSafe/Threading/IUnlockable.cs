namespace Fasciculus.Threading
{
    /// <summary>
    /// Interface implemented by synchronization classes.
    /// </summary>
    public interface IUnlockable
    {
        /// <summary>
        /// Unlocks this synchronization object.
        /// </summary>
        public void Unlock();
    }
}
