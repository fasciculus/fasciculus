using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Fasciculus.IO
{
    public static class Zip
    {
        public static async Task Extract(FileInfo zipFile, DirectoryInfo outputDirectory)
        {
            using Stream stream = zipFile.OpenRead();
            using ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read);

            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                FileInfo outputFile = outputDirectory.File(entry.FullName);

                if (outputFile.Exists) continue;

                outputFile.Directory.Create();

                using Stream entryStream = entry.Open();
                using Stream outputStream = outputFile.Create();

                await entryStream.CopyToAsync(outputStream);
            }
        }
    }
}
