using Fasciculus.Interop;
using static System.Environment;

namespace Fasciculus.IO
{
    public static class Paths
    {
        public static string Personal = GetFolderPath(SpecialFolder.Personal);
        public static string Documents = GetFolderPath(OS.IsWindows ? SpecialFolder.MyDocuments : SpecialFolder.Personal);
    }
}
