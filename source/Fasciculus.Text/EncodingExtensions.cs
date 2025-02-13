using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Fasciculus.Text
{
    /// <summary>
    /// Encoding extensions.
    /// </summary>
    public static class EncodingExtensions
    {
        /// <summary>
        /// Tries to convert the given bytes into a string.
        /// </summary>
#if NETSTANDARD
        public static bool TryGetString(this Encoding encoding, byte[] bytes, out string? result)
#else
        public static bool TryGetString(this Encoding encoding, byte[] bytes, [NotNullWhen(true)] out string? result)
#endif
        {
            return encoding.TryGetString(bytes, 0, bytes.Length, out result);
        }

        /// <summary>
        /// Tries to convert the given bytes (starting from <paramref name="index"/> for <paramref name="count"/> bytes) into a string.
        /// </summary>
#if NETSTANDARD
        public static bool TryGetString(this Encoding encoding, byte[] bytes, int index, int count, out string? result)
#else
        public static bool TryGetString(this Encoding encoding, byte[] bytes, int index, int count, [NotNullWhen(true)] out string? result)
#endif
        {
            result = null;

            try
            {
                result = encoding.GetString(bytes, index, count);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
