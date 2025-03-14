using System.Threading;

namespace Fasciculus.Threading
{
    /// <summary>
    /// Extensions for <see cref="CancellationToken" />
    /// </summary>
    public static partial class CancellationTokenExtensions
    {
        /// <summary>
        /// Returns the given CancellationToken or <see cref="CancellationToken.None"/> if the given token is <c>null</c>.
        /// </summary>
        public static CancellationToken OrNone(this CancellationToken? cancellationToken)
            => cancellationToken ?? CancellationToken.None;
    }
}