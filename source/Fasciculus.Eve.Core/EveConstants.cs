using Fasciculus.IO;
using System.IO;

namespace Fasciculus.Eve
{
    public static class EveConstants
    {
        public static DirectoryInfo DocumentsDirectory => Directories.Documents.Combine("Fasciculus", "Eve").Existing();
    }
}
