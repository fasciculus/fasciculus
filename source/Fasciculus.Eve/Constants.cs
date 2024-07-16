using Fasciculus.IO;
using System.IO;

namespace Fasciculus.Eve
{
    public static class Constants
    {
        public static DirectoryInfo DocumentsDirectory => Directories.Documents.Combine("Fasciculus", "Eve").Existing();
        public static DirectoryInfo DownloadsDirectory => DocumentsDirectory.Combine("Downloads").Existing();
        public static DirectoryInfo DataDirectory => DocumentsDirectory.Combine("Data").Existing();
        public static DirectoryInfo SdeDirectory => DataDirectory.Combine("sde").Existing();
        public static DirectoryInfo BsdDirectory => SdeDirectory.Combine("bsd").Existing();
        public static DirectoryInfo UniverseDirectory => SdeDirectory.Combine("universe").Existing();
    }
}
