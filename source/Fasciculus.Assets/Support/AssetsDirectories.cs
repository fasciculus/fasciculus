using Fasciculus.IO;
using System.IO;

namespace Fasciculus.Assets.Support
{
    public static class AssetsDirectories
    {
        public static DirectoryInfo Documents
            => SpecialDirectories.Documents.Combine("Fasciculus.Assets").CreateIfNotExists();
    }
}
