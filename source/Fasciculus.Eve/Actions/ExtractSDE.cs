using Fasciculus.Eve.IO;
using Fasciculus.IO;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Actions
{
    public static class ExtractSDE
    {
        public static async Task RunAsync()
        {
            FileInfo zipFile = EveDirectories.Downloads.File("sde.zip");
            DirectoryInfo outDirectory = EveDirectories.Data.Combine("SDE").Existing();

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
