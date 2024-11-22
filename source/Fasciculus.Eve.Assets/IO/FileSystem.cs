using Fasciculus.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.IO;

namespace Fasciculus.Eve.IO
{
    public interface IAssetsDirectories
    {
        public DirectoryInfo Documents { get; }
        public DirectoryInfo Downloads { get; }
        public DirectoryInfo Sde { get; }
        public DirectoryInfo Bsd { get; }
    }

    public class AssetsDirectories : IAssetsDirectories
    {
        private readonly ISpecialDirectories specialDirectories;

        public DirectoryInfo Documents => specialDirectories.Documents.Combine("Fasciculus", "Eve.Assets").CreateIfNotExists();
        public DirectoryInfo Downloads => Documents.Combine("Downloads").CreateIfNotExists();
        public DirectoryInfo Sde => Documents.Combine("Sde").CreateIfNotExists();
        public DirectoryInfo Bsd => Sde.Combine("bsd").CreateIfNotExists();

        public AssetsDirectories(ISpecialDirectories specialDirectories)
        {
            this.specialDirectories = specialDirectories;
        }
    }

    public interface IAssetsFiles
    {
        public FileInfo SdeZip { get; }
        public FileInfo NamesYaml { get; }
    }

    public class AssetsFiles : IAssetsFiles
    {
        private readonly IAssetsDirectories assetsDirectories;

        public FileInfo SdeZip => assetsDirectories.Downloads.File("sde.zip");
        public FileInfo NamesYaml => assetsDirectories.Bsd.File("invNames.yaml");

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
