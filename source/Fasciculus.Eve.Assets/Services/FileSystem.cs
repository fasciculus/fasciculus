using Fasciculus.IO;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IAssetsDirectories
    {
        public DirectoryInfo Documents { get; }
        public DirectoryInfo Downloads { get; }
        public DirectoryInfo Sde { get; }
        public DirectoryInfo Bsd { get; }
        public DirectoryInfo Fsd { get; }
    }

    public class AssetsDirectories : IAssetsDirectories
    {
        private readonly ISpecialDirectories specialDirectories;

        public DirectoryInfo Documents => specialDirectories.Documents.Combine("Fasciculus", "Eve.Assets").CreateIfNotExists();
        public DirectoryInfo Downloads => Documents.Combine("Downloads").CreateIfNotExists();
        public DirectoryInfo Sde => Documents.Combine("Sde").CreateIfNotExists();
        public DirectoryInfo Bsd => Sde.Combine("bsd").CreateIfNotExists();
        public DirectoryInfo Fsd => Sde.Combine("fsd").CreateIfNotExists();

        public AssetsDirectories(ISpecialDirectories specialDirectories)
        {
            this.specialDirectories = specialDirectories;
        }
    }

    public interface IAssetsFiles
    {
        public FileInfo SdeZip { get; }
        public FileInfo NamesYaml { get; }
        public FileInfo TypesYaml { get; }
    }

    public class AssetsFiles : IAssetsFiles
    {
        private readonly IAssetsDirectories assetsDirectories;

        public FileInfo SdeZip => assetsDirectories.Downloads.File("sde.zip");
        public FileInfo NamesYaml => assetsDirectories.Bsd.File("invNames.yaml");
        public FileInfo TypesYaml => assetsDirectories.Fsd.File("types.yaml");

        public AssetsFiles(IAssetsDirectories assetsDirectories)
        {
            this.assetsDirectories = assetsDirectories;
        }
    }

    public static class AssetsFileSystemServices
    {
        public static IServiceCollection AddAssetsFileSystem(this IServiceCollection services)
        {
            services.AddSpecialDirectories();

            services.TryAddSingleton<IAssetsDirectories, AssetsDirectories>();
            services.TryAddSingleton<IAssetsFiles, AssetsFiles>();

            return services;
        }
    }
}
