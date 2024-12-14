using Fasciculus.IO;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IAssetsDirectories
    {
        public DirectoryInfo Documents { get; }
        public DirectoryInfo Downloads { get; }

        public DirectoryInfo Sde { get; }
        public DirectoryInfo SteamImages { get; }

        public DirectoryInfo Resources { get; }
        public DirectoryInfo Images { get; }
    }

    public class AssetsDirectories : IAssetsDirectories
    {
        private readonly ISpecialDirectories specialDirectories;

        public DirectoryInfo Documents => specialDirectories.Documents.Combine("Eve.Assets").CreateIfNotExists();
        public DirectoryInfo Downloads => Documents.Combine("Downloads").CreateIfNotExists();

        public DirectoryInfo Sde => Documents.Combine("Sde").CreateIfNotExists();
        public DirectoryInfo SteamImages => Documents.Combine("SteamImages").CreateIfNotExists();

        public DirectoryInfo Resources => Documents.Combine("Resources").CreateIfNotExists();
        public DirectoryInfo Images => Resources.Combine("Images").CreateIfNotExists();

        public AssetsDirectories(ISpecialDirectories specialDirectories)
        {
            this.specialDirectories = specialDirectories;
        }
    }

    public static class AssetsFileSystemServices
    {
        public static IServiceCollection AddAssetsDirectories(this IServiceCollection services)
        {
            services.AddSpecialDirectories();

            services.TryAddSingleton<IAssetsDirectories, AssetsDirectories>();

            return services;
        }
    }
}
