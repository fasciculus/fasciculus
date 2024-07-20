using Fasciculus.IO;
using System.IO;

namespace Fasciculus.Eve
{
    public static class Constants
    {
        public static DirectoryInfo DocumentsDirectory
            => Directories.Documents.Combine("Fasciculus", "Eve.Converter").Existing();

        public static DirectoryInfo DownloadsDirectory
            => DocumentsDirectory.Combine("Downloads").Existing();

        public static DirectoryInfo SdeDirectory
            => DocumentsDirectory.Combine("Sde").Existing();

        public static DirectoryInfo BsdDirectory
            => SdeDirectory.Combine("bsd");

        public static DirectoryInfo ResourcesDirectory
            => DocumentsDirectory.Combine("Resources").Existing();
    }
}
