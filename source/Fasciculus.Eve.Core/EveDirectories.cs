using Fasciculus.IO;
using System.IO;

namespace Fasciculus.Eve
{
    public static class EveDirectories
    {
        public static DirectoryInfo Documents => Directories.Documents.Combine("Fasciculus", "Eve").Existing();
    }
}
