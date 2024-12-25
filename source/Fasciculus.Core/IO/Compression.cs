using System.IO;
using System.IO.Compression;

namespace Fasciculus.IO
{
    public interface ICompression
    {
        public void GZip(Stream uncompressed, Stream compressed);
        public void UnGZip(Stream compressed, Stream uncompressed);
    }

    public class Compression : ICompression
    {
        public void GZip(Stream uncompressed, Stream compressed)
        {
            using GZipStream gzStream = new(compressed, CompressionMode.Compress);

            uncompressed.CopyTo(gzStream);
        }

        public void UnGZip(Stream compressed, Stream uncompressed)
        {
            using GZipStream gzStream = new(compressed, CompressionMode.Decompress);

            gzStream.CopyTo(uncompressed);
        }
    }

    public static class GZip
    {
        public static void Compress(Stream uncompressed, Stream compressed)
        {
            using GZipStream gzStream = new(compressed, CompressionMode.Compress);

            uncompressed.CopyTo(gzStream);
        }

        public static void Compress(Stream uncompressed, FileInfo compressedFile)
        {
            compressedFile.Write(stream => Compress(uncompressed, stream));
        }

        public static void Extract(Stream compressed, Stream uncompressed)
        {
            using GZipStream gzStream = new(compressed, CompressionMode.Decompress);

            gzStream.CopyTo(uncompressed);
        }

        public static void Extract(FileInfo compressedFile, Stream uncompressed)
        {
            using Stream compressed = compressedFile.OpenRead();

            Extract(compressed, uncompressed);
        }

        public static byte[] Extract(FileInfo compressedFile)
        {
            MemoryStream uncompressed = new();

            Extract(compressedFile, uncompressed);

            return uncompressed.ToArray();
        }
    }

}