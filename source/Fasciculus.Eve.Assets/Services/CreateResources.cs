using Fasciculus.Eve.Assets.Models;
using Fasciculus.Eve.Models;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Assets.Services
{
    public interface ICreateResources
    {
        public Task CreateAsync();
    }

    public class CreateResources : ICreateResources
    {
        private readonly IParseData parseData;
        private readonly IConvertUniverse convertUniverse;
        private readonly ICreateImages createImages;

        private readonly IAssetsDirectories assetsDirectories;
        private readonly IWriteResource writeResource;

        private readonly IAssetsProgress progress;

        public CreateResources(IParseData parseData, IConvertUniverse convertUniverse, ICreateImages createImages,
            IAssetsDirectories assetsDirectories, IWriteResource writeResource, IAssetsProgress progress)
        {
            this.parseData = parseData;
            this.convertUniverse = convertUniverse;
            this.createImages = createImages;
            this.assetsDirectories = assetsDirectories;
            this.writeResource = writeResource;
            this.progress = progress;
        }

        private void Create()
        {
            Task<SdeData> data = parseData.ParseAsync();
            Task<EveUniverse> universe = convertUniverse.ConvertAsync();
            Task<List<FileInfo>> images = createImages.CreateAsync();

            Task.WaitAll([data, universe, images]);

            progress.CreateResources.Report(images.Result);
            progress.CreateResources.Report(WriteUniverse(universe.Result));
            progress.CreateResources.Report(WriteVersion(data.Result));
        }

        public Task CreateAsync()
        {
            return Tasks.LongRunning(() => Create());
        }

        private List<FileInfo> WriteVersion(SdeData data)
        {
            using MemoryStream stream = new();
            FileInfo file = assetsDirectories.Resources.File("SdeVersion");

            stream.WriteLong(data.Version.ToBinary());

            return writeResource.Write(stream.ToArray(), file, false) ? [file] : [];
        }

        private List<FileInfo> WriteUniverse(EveUniverse universe)
        {
            using MemoryStream stream = new();
            FileInfo file = assetsDirectories.Resources.File("EveUniverse");

            universe.Write(stream);

            return writeResource.Write(stream.ToArray(), file, true) ? [file] : [];
        }
    }

    public static class CreateResourcesServices
    {
        public static IServiceCollection AddCreateResources(this IServiceCollection services)
        {
            services.AddImages();
            services.AddParseData();
            services.AddConvertUniverse();
            services.AddWriteResource();

            services.TryAddSingleton<ICreateResources, CreateResources>();

            return services;
        }
    }
}
