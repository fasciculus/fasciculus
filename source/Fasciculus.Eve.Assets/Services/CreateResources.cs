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
        private readonly IConvertData convertData;
        private readonly IConvertUniverse convertUniverse;
        private readonly ICreateImages createImages;

        private readonly IAssetsDirectories assetsDirectories;
        private readonly IWriteResource writeResource;

        private readonly IAssetsProgress progress;

        public CreateResources(IConvertData convertData, IConvertUniverse convertUniverse, ICreateImages createImages,
            IAssetsDirectories assetsDirectories, IWriteResource writeResource, IAssetsProgress progress)
        {
            this.convertData = convertData;
            this.convertUniverse = convertUniverse;
            this.createImages = createImages;
            this.assetsDirectories = assetsDirectories;
            this.writeResource = writeResource;
            this.progress = progress;
        }

        private void Create()
        {
            var result = Tasks.Wait(convertData.Data, convertUniverse.ConvertAsync(), createImages.CreateAsync());
            EveData.Data data = result.Item1;
            EveUniverse universe = result.Item2;
            List<FileInfo> images = result.Item3;

            progress.CreateResources.Report(WriteData(data));
            progress.CreateResources.Report(WriteUniverse(universe));
            progress.CreateResources.Report(images);
        }

        public Task CreateAsync()
        {
            return Tasks.LongRunning(() => Create());
        }

        private List<FileInfo> WriteData(EveData.Data data)
        {
            using MemoryStream stream = new();
            FileInfo file = assetsDirectories.Resources.File("EveData");

            data.Write(stream);

            return writeResource.Write(stream.ToArray(), file, true) ? [file] : [];
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
            services.AddConvertData();
            services.AddConvertUniverse();
            services.AddWriteResource();
            services.AddAssetsProgress();

            services.TryAddSingleton<ICreateResources, CreateResources>();

            return services;
        }
    }
}
