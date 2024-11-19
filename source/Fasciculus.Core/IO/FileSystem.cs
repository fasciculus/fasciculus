using Fasciculus.Interop;
using System;
using System.IO;
using static System.Environment;

namespace Fasciculus.IO
{
    public interface ISpecialPaths
    {
        public string Home { get; }
        public string Personal { get; }
        public string Documents { get; }
        public string Downloads { get; }

        public string BaseDirectory { get; }
    }

    public class SpecialPaths : ISpecialPaths
    {
        public string Home => GetFolderPath(SpecialFolder.UserProfile);
        public string Personal => GetFolderPath(SpecialFolder.Personal);
        public string Documents => GetFolderPath(OS.IsWindows ? SpecialFolder.MyDocuments : SpecialFolder.Personal);
        public string Downloads => OS.IsWindows ? Path.Combine(Home, "Downloads") : Home;

        public string BaseDirectory => AppDomain.CurrentDomain.BaseDirectory;
    }
}
