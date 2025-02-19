using System.Buffers;

namespace System.IO
{
    internal static partial class FasciculusStreamExtensions
    {
        internal static void Write(this Stream stream, ReadOnlySpan<byte> buffer)
        {
            byte[] sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);

            try
            {
                buffer.CopyTo(sharedBuffer);
                stream.Write(sharedBuffer, 0, buffer.Length);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(sharedBuffer);
            }
        }
    }
}