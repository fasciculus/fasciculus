using Fasciculus.Interop;
using System.IO;
using static System.Environment;

namespace Fasciculus.IO
{
    public static class Folders
    {
        public static DirectoryInfo Personal => new(GetFolderPath(SpecialFolder.Personal));
        public static DirectoryInfo Documents => new(GetFolderPath(OSTypes.IsWindows ? SpecialFolder.MyDocuments : SpecialFolder.Personal));
    }
}
