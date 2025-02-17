namespace System.IO
{
    // available in net7.0

    internal static partial class FasciculusStreamExtensions
    {
        internal static int ReadAtLeast(this Stream stream, Span<byte> buffer, int minimumBytes, bool throwOnEndOfStream = true)
        {
            return ReadAtLeastCore(stream, buffer, minimumBytes, throwOnEndOfStream);
        }
    }
}