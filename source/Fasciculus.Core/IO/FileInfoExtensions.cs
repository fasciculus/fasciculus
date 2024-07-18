using System.Text;

namespace System.IO
{
    public static class FileInfoExtensions
    {
        public static bool IsNewerThan(this FileInfo file, FileInfo target)
            => !target.Exists || file.LastWriteTimeUtc > target.LastWriteTimeUtc;

        public static string ReadAllText(this FileInfo file, Encoding encoding)
            => File.ReadAllText(file.FullName, Encoding.UTF8);

        public static string ReadAllText(this FileInfo file)
            => File.ReadAllText(file.FullName, Encoding.UTF8);
    }
}
