using System.Text;

namespace System.IO
{
    public enum FileOverwriteMode
    {
        Never,
        IfNewer,
        Always
    }

    public static class FileInfoExtensions
    {
        public static void DeleteIfExists(this FileInfo file)
        {
            if (File.Exists(file.FullName))
            {
                file.Delete();
            }
        }

        public static bool IsNewerThan(this FileInfo file, DateTime lastWriteTimeUtc)
            => file.LastWriteTimeUtc > lastWriteTimeUtc;

        public static bool IsNewerThan(this FileInfo file, FileInfo target)
            => !target.Exists || file.IsNewerThan(target.LastWriteTimeUtc);

        public static bool NeedsOverwrite(this FileInfo file, DateTime dateTimeUtc, FileOverwriteMode mode)
        {
            return mode switch
            {
                FileOverwriteMode.Never => !file.Exists,
                FileOverwriteMode.IfNewer => !file.Exists || !file.IsNewerThan(dateTimeUtc),
                FileOverwriteMode.Always => true,
                _ => false
            };
        }

        public static string ReadAllText(this FileInfo file, Encoding? encoding = null)
            => File.ReadAllText(file.FullName, encoding ?? Encoding.UTF8);

        public static byte[] ReadAllBytes(this FileInfo file)
            => File.ReadAllBytes(file.FullName);

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
