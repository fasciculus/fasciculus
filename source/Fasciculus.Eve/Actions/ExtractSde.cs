using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Actions
{
    public static class ExtractSde
    {
        public static async Task RunAsync()
        {
            FileInfo zipFile = Constants.DownloadsDirectory.File("sde.zip");
            DirectoryInfo outDirectory = Constants.SdeDirectory;

            using Stream zipStream = new FileStream(zipFile.FullName, FileMode.Open, FileAccess.Read);
            using ZipArchive zip = new(zipStream);

            foreach (ZipArchiveEntry entry in zip.Entries)
            {
                FileInfo outFile = outDirectory.File(entry.FullName);
                DirectoryInfo? parentDirectory = outFile.Directory?.Existing();

                if (outFile.Exists)
                {
                    continue;
                }

                using Stream inStream = entry.Open();
                using Stream outStream = outFile.Create();

                await inStream.CopyToAsync(outStream);
            }
        }
    }
}
