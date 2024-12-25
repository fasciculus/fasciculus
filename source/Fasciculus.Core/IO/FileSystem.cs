using System.IO;

namespace Fasciculus.IO
{
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
        public DirectoryInfo Home => new(SpecialPaths.Home);
        public DirectoryInfo Personal => new(SpecialPaths.Personal);
        public DirectoryInfo Documents => new(SpecialPaths.Documents);
        public DirectoryInfo Downloads => new(SpecialPaths.Downloads);

        public DirectoryInfo BaseDirectory => new(SpecialPaths.BaseDirectory);
    }

    public static class FileSystemServices
    {
    }
}