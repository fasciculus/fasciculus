using Fasciculus.IO;
using System.IO;

namespace Fasciculus.Eve
{
    public static class EveAssetsDirectories
    {
        public static DirectoryInfo DocumentsDirectory => Directories.Documents.Combine("Fasciculus", "Eve.Assets").Existing();
        public static DirectoryInfo DownloadsDirectory => DocumentsDirectory.Combine("Downloads").Existing();
        public static DirectoryInfo SdeDirectory => DocumentsDirectory.Combine("Sde").Existing();
        public static DirectoryInfo BsdDirectory => SdeDirectory.Combine("bsd").Existing();
        public static DirectoryInfo UniverseDirectory => SdeDirectory.Combine("universe").Existing();
        public static DirectoryInfo RegionsDirectory => UniverseDirectory.Combine("eve").Existing();
    }
}
