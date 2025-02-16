namespace System.IO
{
    internal static partial class FasciculusNetStandardStreamExtensions
    {
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