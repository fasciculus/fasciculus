using System.IO;

namespace Fasciculus.IO
{
    public static class Directories
    {
        public static DirectoryInfo Personal => new(Paths.Personal);
        public static DirectoryInfo Documents => new(Paths.Documents);
        public static DirectoryInfo Downloads => new(Paths.Downloads);
    }
}
