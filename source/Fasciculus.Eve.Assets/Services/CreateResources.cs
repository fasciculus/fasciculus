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
        private readonly ICreateNavigation createNavigation;
        private readonly ICreateImages createImages;

        private readonly IAssetsDirectories assetsDirectories;
        private readonly IWriteResource writeResource;

        private readonly IAssetsProgress progress;

        public CreateResources(IConvertData convertData, IConvertUniverse convertUniverse, ICreateNavigation createNavigation,
            ICreateImages createImages, IAssetsDirectories assetsDirectories, IWriteResource writeResource, IAssetsProgress progress)
        {
            this.convertData = convertData;
            this.convertUniverse = convertUniverse;
            this.createNavigation = createNavigation;
            this.createImages = createImages;
            this.assetsDirectories = assetsDirectories;
            this.writeResource = writeResource;
            this.progress = progress;
        }

        private void Create()
        {
            var result = Tasks.Wait(convertData.Data, convertUniverse.Universe, createNavigation.Navigation, createImages.FilesCreated);
            EveData.Data data = result.Item1;
            EveUniverse.Data universe = result.Item2;
            EveNavigation.Data navigation = result.Item3;
            List<FileInfo> images = result.Item4;

            progress.CreateResources.Report(WriteData(data));
            progress.CreateResources.Report(WriteUniverse(universe));
            progress.CreateResources.Report(WriteNavigation(navigation));
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

        private List<FileInfo> WriteUniverse(EveUniverse.Data universe)
        {
            using MemoryStream stream = new();
            FileInfo file = assetsDirectories.Resources.File("EveUniverse");

            universe.Write(stream);

            return writeResource.Write(stream.ToArray(), file, true) ? [file] : [];
        }

        private List<FileInfo> WriteNavigation(EveNavigation.Data navigation)
        {
            using MemoryStream stream = new();
            FileInfo file = assetsDirectories.Resources.File("EveNavigation");

            navigation.Write(stream);

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
            services.AddCreateNanigation();
            services.AddWriteResource();
            services.AddAssetsProgress();

            services.TryAddSingleton<ICreateResources, CreateResources>();

            return services;
        }
    }
}
