using System.Text;

namespace System.IO
{
    public static class FileInfoExtensions
    {
        public static void DeleteIfExists(this FileInfo file)
        {
            if (File.Exists(file.FullName))
            {
                file.Delete();
            }
        }

        public static bool IsNewerThan(this FileInfo file, FileInfo target)
            => !target.Exists || file.LastWriteTimeUtc > target.LastWriteTimeUtc;

        public static string ReadAllText(this FileInfo file, Encoding encoding)
            => File.ReadAllText(file.FullName, Encoding.UTF8);

        public static string ReadAllText(this FileInfo file)
            => File.ReadAllText(file.FullName, Encoding.UTF8);

        public static void WriteAllBytes(this FileInfo file, byte[] bytes)
            => File.WriteAllBytes(file.FullName, bytes);

        public static void Read(this FileInfo file, Action<Stream> read)
        {
            using Stream stream = file.OpenRead();

            read(stream);
        }

        public static FileInfo Write(this FileInfo file, Action<Stream> write)
        {
            file.DeleteIfExists();

            using (Stream stream = file.Create())
            {
                write(stream);
            }

            return new(file.FullName);
        }
    }
}
