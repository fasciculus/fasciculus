using Fasciculus.IO;
using System.IO;

namespace Fasciculus.Eve.IO
{
    public static class EveDirectories
    {
        public static DirectoryInfo Documents => Directories.Documents.Combine("Fasciculus", "Eve").Existing();
        public static DirectoryInfo Downloads => Documents.Combine("Downloads").Existing();
        public static DirectoryInfo Data => Documents.Combine("Data").Existing();
    }
}
