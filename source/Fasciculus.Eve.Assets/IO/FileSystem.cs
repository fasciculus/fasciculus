using Fasciculus.IO;
using System.IO;

namespace Fasciculus.Eve.IO
{
    public interface IAssetDirectories
    {
        public DirectoryInfo Documents { get; }
        public DirectoryInfo Downloads { get; }
    }

    public class AssetDirectories : IAssetDirectories
    {
        private readonly ISpecialDirectories specialDirectories;

        public DirectoryInfo Documents => specialDirectories.Documents.Combine("Fasciculus", "Eve.Assets").CreateIfNotExists();
        public DirectoryInfo Downloads => Documents.Combine("Downloads").CreateIfNotExists();

        public AssetDirectories(ISpecialDirectories specialDirectories)
        {
            this.specialDirectories = specialDirectories;
        }
    }

    public interface IAssetFiles
    {
        public FileInfo SdeZip { get; }
    }

    public class AssetFiles : IAssetFiles
    {
        private readonly IAssetDirectories assetDirectories;

        public FileInfo SdeZip => assetDirectories.Downloads.File("sde.zip");

        public AssetFiles(IAssetDirectories assetDirectories)
        {
            this.assetDirectories = assetDirectories;
        }
    }
}
