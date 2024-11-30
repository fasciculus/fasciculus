﻿using Fasciculus.Algorithms;
using Fasciculus.Eve.Assets.Models;
using Fasciculus.IO;
using Fasciculus.Threading;
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

    public interface ICreateResources
    {
        public void Create();
    }

    public class CreateResources : ICreateResources
    {
        private readonly IParseData parseData;
        private readonly IParseUniverse parseUniverse;
        private readonly ICopyImages copyImages;

        private readonly IAssetsDirectories assetsDirectories;
        private readonly IWriteResource writeResource;

        private readonly IAssetsProgress progress;

        public CreateResources(IParseData parseData, IParseUniverse parseUniverse, ICopyImages copyImages,
            IAssetsDirectories assetsDirectories, IWriteResource writeResource, IAssetsProgress progress)
        {
            this.parseData = parseData;
            this.parseUniverse = parseUniverse;
            this.copyImages = copyImages;
            this.assetsDirectories = assetsDirectories;
            this.writeResource = writeResource;
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

            if (writeResource.Write(stream.ToArray(), file, false))
            {
                progress.CreateResources.Report([file]);
            }
        }

        private SdeData ParseSdeData()
            => parseData.Parse();

        private SdeRegion[] ParseUniverse()
            => parseUniverse.Parse();

        private void CopyImages()
            => copyImages.Copy();
    }

    public static class ResourcesServices
    {
        public static IServiceCollection AddWriteResource(this IServiceCollection services)
        {
            services.AddCompression();

            services.TryAddSingleton<IWriteResource, WriteResource>();

            return services;
        }

        public static IServiceCollection AddCreateResources(this IServiceCollection services)
        {
            services.AddImages();
            services.AddDataParsers();
            services.AddUniverseParser();
            services.AddWriteResource();

            services.TryAddSingleton<ICreateResources, CreateResources>();

            return services;
        }
    }
}
