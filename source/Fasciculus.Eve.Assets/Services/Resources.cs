using Fasciculus.Algorithms;
using Fasciculus.Eve.Assets.Models;
using Fasciculus.IO;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IResourceWriter
    {
        public bool Write(byte[] source, FileInfo destination, bool compressed);
        public bool Copy(FileInfo source, FileInfo destination, bool compressed);
    }

    public class ResourceWriter : IResourceWriter
    {
        private readonly ICompression compression;

        public ResourceWriter(ICompression compression)
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

    public interface IResourcesCreator
    {
        public void Create();
    }

    public class ResourcesCreator : IResourcesCreator
    {
        private readonly IDataParser dataParser;
        private readonly IUniverseParser universeParser;
        private readonly IImageCopier imageCopier;

        private readonly IAssetsDirectories assetsDirectories;
        private readonly IResourceWriter resourceWriter;

        private readonly IProgress<FileInfo> progress;

        public ResourcesCreator(IDataParser dataParser, IUniverseParser universeParser, IImageCopier imageCopier,
            IAssetsDirectories assetsDirectories, IResourceWriter resourceWriter,
            [FromKeyedServices(ServiceKeys.ResourcesCreator)] IProgress<FileInfo> progress)
        {
            this.dataParser = dataParser;
            this.universeParser = universeParser;
            this.imageCopier = imageCopier;
            this.assetsDirectories = assetsDirectories;
            this.resourceWriter = resourceWriter;
            this.progress = progress;
        }

        public void Create()
        {
            Task<SdeData> parseSdeData = Tasks.Start(ParseSdeData);
            Task<SdeRegion[]> parseUniverse = Tasks.Start(ParseUniverse);
            Task extractImages = Tasks.Start(CopyImages);

            Task.WaitAll([parseSdeData, parseUniverse, extractImages]);

            WriteVersion(parseSdeData.Result);
        }

        private void WriteVersion(SdeData sdeData)
        {
            using MemoryStream stream = new();
            FileInfo file = assetsDirectories.Resources.File("SdeVersion");

            stream.WriteLong(sdeData.Version.ToBinary());

            if (resourceWriter.Write(stream.ToArray(), file, false))
            {
                progress.Report(file);
            }
        }

        private SdeData ParseSdeData()
            => dataParser.Parse();

        private SdeRegion[] ParseUniverse()
            => universeParser.Parse();

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
            services.AddUniverseParser();
            services.AddResourceWriter();

            services.TryAddSingleton<IResourcesCreator, ResourcesCreator>();

            return services;
        }
    }
}
