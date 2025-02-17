namespace System.IO
{
    // available in net7.0

    internal static partial class FasciculusNetStandardStreamExtensions
    {
        internal static void ReadExactly(this Stream stream, Span<byte> buffer)
        {
            _ = ReadAtLeastCore(stream, buffer, buffer.Length, true);
        }

        internal static void ReadExactly(this Stream stream, byte[] buffer, int offset, int count)
        {
            _ = ReadAtLeastCore(stream, buffer.AsSpan(offset, count), count, true);
        }

    }
}