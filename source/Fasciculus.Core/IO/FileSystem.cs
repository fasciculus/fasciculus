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

    public interface ISpecialDirectories
    {
        public DirectoryInfo Home { get; }
        public DirectoryInfo Personal { get; }
        public DirectoryInfo Documents { get; }
        public DirectoryInfo Downloads { get; }

        public DirectoryInfo BaseDirectory { get; }
    }

    public class SpecialDirectories : ISpecialDirectories
    {
        protected ISpecialPaths specialPaths;

        public DirectoryInfo Home => new(specialPaths.Home);
        public DirectoryInfo Personal => new(specialPaths.Personal);
        public DirectoryInfo Documents => new(specialPaths.Documents);
        public DirectoryInfo Downloads => new(specialPaths.Downloads);

        public DirectoryInfo BaseDirectory => new(specialPaths.BaseDirectory);

        public SpecialDirectories(ISpecialPaths specialPaths)
        {
            this.specialPaths = specialPaths;
        }
    }

    public static class FileSystemServices
    {
    }
}