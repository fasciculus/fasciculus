using System.IO;

namespace Fasciculus.IO
{
    public static class DirectoryInfoExtensions
    {
        public static DirectoryInfo Combine(this DirectoryInfo directory, params string[] paths)
        {
            string result = directory.FullName;

            foreach (string path in paths)
            {
                result = Path.Combine(result, path);
            }

            return new DirectoryInfo(result);
        }

        public static FileInfo File(this DirectoryInfo directory, string name)
        {
            return new(Path.Combine(directory.FullName, name));
        }

        public static DirectoryInfo Make(this DirectoryInfo directory)
        {
            directory.Create();

            return new(directory.FullName);
        }
    }
}
