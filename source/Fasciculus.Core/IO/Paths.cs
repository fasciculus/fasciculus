using Fasciculus.Interop;
using System;
using System.IO;
using static System.Environment;

namespace Fasciculus.IO
{
    public static class Paths
    {
        public static string Personal
            => GetFolderPath(SpecialFolder.Personal);

        public static string Documents
            => GetFolderPath(OS.IsWindows ? SpecialFolder.MyDocuments : SpecialFolder.Personal);

        public static string Downloads
            => OS.IsWindows ? Path.Combine(Personal, "Downloads") : Personal;

        public static string BaseDirectory
            => AppDomain.CurrentDomain.BaseDirectory;
    }
}
