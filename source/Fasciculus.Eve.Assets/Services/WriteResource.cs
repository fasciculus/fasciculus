using Fasciculus.Algorithms;
using Fasciculus.IO.Compressing;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IWriteResource
    {
        public bool Write(byte[] source, FileInfo destination, bool compressed);
        public bool Copy(FileInfo source, FileInfo destination, bool compressed);
    }

    public class WriteResource : IWriteResource
    {
        public bool Copy(FileInfo source, FileInfo destination, bool compress)
        {
            byte[] bytes = source.ReadAllBytes();

            return Write(bytes, destination, compress);
        }

        public bool Write(byte[] source, FileInfo destination, bool compress)
        {
            bool writeRequired = WriteRequired(source, destination, compress);

            if (writeRequired)
            {
                destination.Directory?.CreateIfNotExists();
                destination.DeleteIfExists();

                if (compress)
                {
                    using Stream uncompressed = new MemoryStream(source);
                    using Stream compressed = destination.Open(FileMode.CreateNew);

                    GZip.Compress(uncompressed, compressed);
                }
                else
                {
                    destination.WriteAllBytes(source);
                }
            }

            return writeRequired;
        }

        private static bool WriteRequired(byte[] source, FileInfo destination, bool compress)
        {
            bool result = !destination.Exists;

            if (!result)
            {
                byte[] existing = Read(destination, compress);

                result = !Equality.AreEqual(source, existing);
            }

            return result;
        }

        private static byte[] Read(FileInfo destination, bool compress)
        {
            byte[] result = destination.ReadAllBytes();

            if (compress)
            {
                using MemoryStream compressed = new(result);
                using MemoryStream uncompressed = new();

                GZip.Extract(compressed, uncompressed);
                result = uncompressed.ToArray();
            }

            return result;
        }
    }
}
