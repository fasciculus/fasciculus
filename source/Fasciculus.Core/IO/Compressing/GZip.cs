using System.IO;
using System.IO.Compression;

namespace Fasciculus.IO.Compressing
{
    /// <summary>
    /// GZip support
    /// </summary>
    public static class GZip
    {
        /// <summary>
        /// Compresses the given <paramref name="uncompressed"/> stream into the given <paramref name="compressed"/> stream.
        /// </summary>
        public static void Compress(Stream uncompressed, Stream compressed)
        {
            using GZipStream gZipStream = new(compressed, CompressionMode.Compress);

            uncompressed.CopyTo(gZipStream);
        }

        /// <summary>
        /// Extracts the given <paramref name="compressed"/> stream into the given <paramref name="uncompressed"/> stream.
        /// </summary>
        public static void Extract(Stream compressed, Stream uncompressed)
        {
            using GZipStream gZipStream = new(compressed, CompressionMode.Decompress);

            gZipStream.CopyTo(uncompressed);
        }
    }
}
