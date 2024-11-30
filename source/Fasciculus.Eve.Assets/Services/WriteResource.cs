using Fasciculus.Algorithms;
using Fasciculus.IO;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IWriteResource
    {
        public bool Write(byte[] source, FileInfo destination, bool compressed);
        public bool Copy(FileInfo source, FileInfo destination, bool compressed);
    }

    public class WriteResource : IWriteResource
    {
        private readonly ICompression compression;

        public WriteResource(ICompression compression)
        {
            this.compression = compression;
        }

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

                    compression.GZip(uncompressed, compressed);
                }
                else
                {
                    destination.WriteAllBytes(source);
                }
            }

            return writeRequired;
        }

        private bool WriteRequired(byte[] source, FileInfo destination, bool compress)
        {
            bool result = !destination.Exists;

            if (!result)
            {
                byte[] existing = Read(destination, compress);

                result = !Equality.AreEqual(source, existing);
            }

            return result;
        }

        private byte[] Read(FileInfo destination, bool compress)
        {
            byte[] result = destination.ReadAllBytes();

            if (compress)
            {
                using MemoryStream compressed = new(result);
                using MemoryStream uncompressed = new();

                compression.UnGZip(compressed, uncompressed);
                result = uncompressed.ToArray();
            }

            return result;
        }
    }

    public static class WriteResourceServices
    {
        public static IServiceCollection AddWriteResource(this IServiceCollection services)
        {
            services.AddCompression();

            services.TryAddSingleton<IWriteResource, WriteResource>();

            return services;
        }
    }
}
