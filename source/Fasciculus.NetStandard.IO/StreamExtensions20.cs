using System.Buffers;

namespace System.IO
{
    internal static class StreamExtensions20
    {
        public static int Read(this Stream stream, Span<byte> buffer)
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

        public static void ReadExactly(this Stream stream, Span<byte> buffer)
        {
            _ = ReadAtLeastCore(stream, buffer, buffer.Length, true);
        }

        public static void ReadExactly(this Stream stream, byte[] buffer, int offset, int count)
        {
            _ = ReadAtLeastCore(stream, buffer.AsSpan(offset, count), count, true);
        }

        public static int ReadAtLeast(this Stream stream, Span<byte> buffer, int minimumBytes, bool throwOnEndOfStream = true)
        {
            return ReadAtLeastCore(stream, buffer, minimumBytes, throwOnEndOfStream);
        }

        private static int ReadAtLeastCore(Stream stream, Span<byte> buffer, int minimumBytes, bool throwOnEndOfStream)
        {
            int totalRead = 0;

            while (totalRead < minimumBytes)
            {
                int read = stream.Read(buffer.Slice(totalRead));

                if (read == 0)
                {
                    if (throwOnEndOfStream)
                    {
                        throw new EndOfStreamException();
                    }

                    return totalRead;
                }

                totalRead += read;
            }

            return totalRead;
        }
    }
}
