using Fasciculus.IO;
using System.IO;

namespace Fasciculus.Eve.IO
{
    public static class EveFolders
    {
        public static DirectoryInfo EveDocuments
        {
            get
            {
                return new(Path.Combine(Folders.Documents.FullName, "Eve"));
            }
        }
    }
}
