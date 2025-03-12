using System.Threading;

namespace Fasciculus.Threading
{
    /// <summary>
    /// Interlocked value.
    /// </summary>
    public abstract class InterlockedValue<T>
    {
        private long encoded;

        /// <summary>
        /// Encodes the given value to a long.
        /// </summary>
        protected abstract long Encode(T value);

        /// <summary>
        /// Decodes the given value.
        /// </summary>
        protected abstract T Decode(long value);

        /// <summary>
        /// Reads the value.
        /// </summary>
        public T Read()
            => Decode(Interlocked.Read(ref encoded));

        /// <summary>
        /// Exchanges the stored value with the given <paramref name="value"/>.
        /// </summary>
        public void Write(T value)
            => Interlocked.Exchange(ref encoded, Encode(value));

        /// <summary>
        /// Compares the stored value with the given <paramref name="comparand"/>. If they are equal, the stored value gets
        /// exchanged with the given value.
        /// </summary>
        /// <returns>Whether the value was exchanged.</returns>
        public bool Replace(T value, T comparand)
        {
            long encodedValue = Encode(value);
            long encodedComparand = Encode(comparand);

            return Interlocked.CompareExchange(ref encoded, encodedValue, encodedComparand) == encodedComparand;
        }

        /// <summary>
        /// Increments the stored value.
        /// </summary>
        public T Increment()
            => Decode(Interlocked.Increment(ref encoded));

        /// <summary>
        /// Decrements the stored value.
        /// </summary>
        public T Decrement()
            => Decode(Interlocked.Decrement(ref encoded));

        /// <summary>
        /// Adds the given value
        /// </summary>
        public T Add(T value)
            => Decode(Interlocked.Add(ref encoded, Encode(value)));

    }
}