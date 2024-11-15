using Fasciculus.IO;
using System.IO;

namespace Fasciculus.Eve
{
    public static class EveFileSystemInfos
    {
        public static DirectoryInfo Documents => Directories.Documents.Combine("Fasciculus", "Eve").Existing();

        public static FileInfo EsiCacheFile => Documents.File("Esi.cache");
    }
}
