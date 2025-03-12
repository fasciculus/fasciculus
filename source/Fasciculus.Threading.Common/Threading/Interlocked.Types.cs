namespace Fasciculus.Threading
{
    /// <summary>
    /// Interlocked <c>bool</c>.
    /// </summary>
    public class InterlockedBool : InterlockedValue<bool>
    {
        /// <summary>
        /// Decodes the given value.
        /// </summary>
        protected override bool Decode(long value)
            => value != 0;

        /// <summary>
        /// Encodes the given value to a long.
        /// </summary>
        protected override long Encode(bool value)
            => value ? 1 : 0;
    }

    /// <summary>
    /// Interlocked <c>long</c>.
    /// </summary>
    public class InterlockedLong : InterlockedValue<long>
    {
        /// <summary>
        /// Decodes the given value.
        /// </summary>
        protected override long Decode(long value)
            => value;

        /// <summary>
        /// Encodes the given value to a long.
        /// </summary>
        protected override long Encode(long value)
            => value;
    }
}