using System.Text;

namespace Fasciculus.IO
{
    /// <summary>
    /// Extensions for <see cref="Encoding"/>.
    /// </summary>
    public static partial class EncodingExtensions
    {
        /// <summary>
        /// Returns UTF-8 if the given <paramref name="encoding"/> is <c>null</c>.
        /// </summary>
        public static Encoding OrUTF8(this Encoding? encoding)
            => encoding ?? Encoding.UTF8;
    }
}