using Fasciculus.IO;
using System.IO;

namespace Fasciculus.Eve.IO
{
    public interface IAssetsDirectories
    {
        public DirectoryInfo Documents { get; }
        public DirectoryInfo Downloads { get; }
    }

    public class AssetsDirectories : IAssetsDirectories
    {
        private readonly ISpecialDirectories specialDirectories;

        public DirectoryInfo Documents => specialDirectories.Documents.Combine("Fasciculus", "Eve.Assets").CreateIfNotExists();
        public DirectoryInfo Downloads => Documents.Combine("Downloads").CreateIfNotExists();

        public AssetsDirectories(ISpecialDirectories specialDirectories)
        {
            this.specialDirectories = specialDirectories;
        }
    }

    public interface IAssetsFiles
    {
        public FileInfo SdeZip { get; }
    }

    public class AssetsFiles : IAssetsFiles
    {
        private readonly IAssetsDirectories assetsDirectories;

        public FileInfo SdeZip => assetsDirectories.Downloads.File("sde.zip");

        public AssetsFiles(IAssetsDirectories assetsDirectories)
        {
            this.assetsDirectories = assetsDirectories;
        }
    }
}
