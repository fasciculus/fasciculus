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

        public static bool IsOlderThan(this FileInfo file, DateTime lastWriteTimeUtc)
            => file.LastWriteTimeUtc < lastWriteTimeUtc;

        public static bool NeedsOverwrite(this FileInfo file, DateTime dateTimeUtc, FileOverwriteMode mode)
        {
            return mode switch
            {
                FileOverwriteMode.Never => !file.Exists,
                FileOverwriteMode.IfNewer => !file.Exists || file.IsOlderThan(dateTimeUtc),
                FileOverwriteMode.Always => true,
                _ => false
            };
        }

        public static string ReadAllText(this FileInfo file, Encoding? encoding = null)
            => File.ReadAllText(file.FullName, encoding ?? Encoding.UTF8);

        public static void WriteAllText(this FileInfo file, string text, Encoding? encoding = null)
            => File.WriteAllText(file.FullName, text, encoding ?? Encoding.UTF8);

        public static string[] ReadAllLines(this FileInfo file, Encoding? encoding = null)
            => File.ReadAllLines(file.FullName, encoding ?? Encoding.UTF8);

        public static byte[] ReadAllBytes(this FileInfo file)
            => File.ReadAllBytes(file.FullName);

        public static void WriteAllBytes(this FileInfo file, byte[] bytes)
            => File.WriteAllBytes(file.FullName, bytes);

        public static void Read(this FileInfo file, Action<Stream> read)
        {
            using Stream stream = file.OpenRead();

            read(stream);
        }

        public static T Read<T>(this FileInfo file, Func<Stream, T> read)
        {
            using Stream stream = file.OpenRead();

            return read(stream);
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
