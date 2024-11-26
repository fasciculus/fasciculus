using Fasciculus.Algorithms;
using Fasciculus.Eve.Assets.Models;
using Fasciculus.IO;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IResourceWriter
    {
        public void Write(byte[] source, FileInfo destination, bool compressed);
        public void Copy(FileInfo source, FileInfo destination, bool compressed);
    }

    public class ResourceWriter : IResourceWriter
    {
        private readonly ICompression compression;

        public ResourceWriter(ICompression compression)
        {
            this.compression = compression;
        }

        public void Copy(FileInfo source, FileInfo destination, bool compress)
        {
            byte[] bytes = source.ReadAllBytes();

            Write(bytes, destination, compress);
        }

        public void Write(byte[] source, FileInfo destination, bool compress)
        {
            if (WriteRequired(source, destination, compress))
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

    public interface IResourcesCreator
    {
        public void Create();
    }

    public class ResourcesCreator : IResourcesCreator
    {
        private readonly IDataParser dataParser;
        private readonly IImageCopier imageCopier;

        public ResourcesCreator(IDataParser dataParser, IImageCopier imageCopier)
        {
            this.dataParser = dataParser;
            this.imageCopier = imageCopier;
        }

        public void Create()
        {
            Task<SdeData> parseSdeData = Tasks.LongRunning(ParseSdeData);
            Task extractImages = Tasks.LongRunning(CopyImages);

            Task.WaitAll([parseSdeData, extractImages]);
        }

        private SdeData ParseSdeData()
            => dataParser.Parse();

        private void CopyImages()
            => imageCopier.Copy();
    }

    public static class ResourcesServices
    {
        public static IServiceCollection AddResourceWriter(this IServiceCollection services)
        {
            services.AddCompression();

            services.TryAddSingleton<IResourceWriter, ResourceWriter>();

            return services;
        }

        public static IServiceCollection AddResourcesCreator(this IServiceCollection services)
        {
            services.AddImages();
            services.AddDataParsers();
            services.AddResourceWriter();

            services.TryAddSingleton<IResourcesCreator, ResourcesCreator>();

            return services;
        }
    }
}
