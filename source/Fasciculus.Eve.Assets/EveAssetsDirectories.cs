using Fasciculus.IO;
using System.IO;

namespace Fasciculus.Eve
{
    public static class EveAssetsDirectories
    {
        public static DirectoryInfo Documents => Directories.Documents.Combine("Fasciculus", "Eve.Assets").Existing();
        public static DirectoryInfo Downloads => Documents.Combine("Downloads").Existing();
    }
}
