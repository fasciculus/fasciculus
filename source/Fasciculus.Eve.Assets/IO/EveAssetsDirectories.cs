using Fasciculus.IO;
using System.IO;

namespace Fasciculus.Eve.IO
{
    public static class EveAssetsDirectories
    {
        public static DirectoryInfo DocumentsDirectory => Directories.Documents.Combine("Fasciculus", "Eve.Assets").CreateIfNotExists();
        public static DirectoryInfo DownloadsDirectory => DocumentsDirectory.Combine("Downloads").CreateIfNotExists();

        public static DirectoryInfo SdeDirectory => DocumentsDirectory.Combine("Sde").CreateIfNotExists();
        public static DirectoryInfo BsdDirectory => SdeDirectory.Combine("bsd").CreateIfNotExists();
        public static DirectoryInfo FsdDirectory => SdeDirectory.Combine("fsd").CreateIfNotExists();
        public static DirectoryInfo UniverseDirectory => SdeDirectory.Combine("universe").CreateIfNotExists();
        public static DirectoryInfo RegionsDirectory => UniverseDirectory.Combine("eve").CreateIfNotExists();

        public static DirectoryInfo ResourcesDirectory => DocumentsDirectory.Combine("Resources").CreateIfNotExists();
    }
}
