using System.Buffers;

namespace System.IO
{
    // available in netstandard2.1

    internal static partial class FasciculusStreamExtensions
    {
        internal static int Read(this Stream stream, Span<byte> buffer)
        {
            byte[] sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);

            try
            {
                int numRead = stream.Read(sharedBuffer, 0, buffer.Length);

                if ((uint)numRead > (uint)buffer.Length)
                {
                    throw new IOException();
                }

                new ReadOnlySpan<byte>(sharedBuffer, 0, numRead).CopyTo(buffer);

                return numRead;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(sharedBuffer);
            }
        }
    }
}