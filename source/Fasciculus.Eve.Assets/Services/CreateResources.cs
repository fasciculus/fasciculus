using Fasciculus.Collections;
using Fasciculus.Eve.Models;
using Fasciculus.IO;
using Fasciculus.Threading;

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

        private readonly ChangedResourcesSet changedResources;

        public CreateResources(IConvertData convertData, IConvertUniverse convertUniverse, ICreateNavigation createNavigation,
            ICreateImages createImages, IAssetsDirectories assetsDirectories, IWriteResource writeResource, ChangedResourcesSet changedResources)
        {
            this.convertData = convertData;
            this.convertUniverse = convertUniverse;
            this.createNavigation = createNavigation;
            this.createImages = createImages;
            this.assetsDirectories = assetsDirectories;
            this.writeResource = writeResource;
            this.changedResources = changedResources;
        }

        private void Create()
        {
            Task<EveData.Data> data = convertData.Data;
            Task<EveUniverse.Data> universe = convertUniverse.Universe;
            Task<EveNavigation.Data> navigation = createNavigation.Navigation;
            Task<List<FileInfo>> images = createImages.FilesCreated;

            Task.WaitAll([data, universe, navigation, images]);

            WriteData(data.Result).Apply(changedResources.Add);
            WriteUniverse(universe.Result).Apply(changedResources.Add);
            WriteNavigation(navigation.Result).Apply(changedResources.Add);
            images.Result.Apply(changedResources.Add);
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
}
