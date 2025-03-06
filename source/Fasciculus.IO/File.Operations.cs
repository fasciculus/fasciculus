using System.IO;

namespace Fasciculus.IO
{
    public static partial class FileInfoExtensions
    {
        /// <summary>
        /// Deletes the given <paramref name="file"/> if it exists.
        /// </summary>
        public static FileInfo DeleteIfExists(this FileInfo file)
        {
            if (file.Update().Exists)
            {
                file.Delete();
            }

            return file.Update();
        }

        /// <summary>
        /// Creates the given <paramref name="file"/>'s parent directory if it doesn't exist.
        /// </summary>
        public static FileInfo EnsureDirectory(this FileInfo file)
        {
            file.Directory?.CreateIfNotExists();

            return file.Update();
        }

    }
}