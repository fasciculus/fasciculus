using System;

namespace Fasciculus.Progress
{
    /// <summary>
    /// An accumulating progress.
    /// </summary>
    public interface IAccumulatingProgress<T> : IProgress<T>
    {
        /// <summary>
        /// Starts the progress with the given <paramref name="total"/> end value.
        /// </summary>
        public void Begin(T total);

        /// <summary>
        /// Ends the progress.
        /// </summary>
        public void End();
    }
}
