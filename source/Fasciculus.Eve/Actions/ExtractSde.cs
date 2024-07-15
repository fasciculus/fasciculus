using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Actions
{
    public static class ExtractSde
    {
        public static async Task RunAsync()
        {

            DirectoryInfo outDirectory = Constants.SdeDirectory;
            FileInfo doneFile = outDirectory.File("done");

            if (doneFile.Exists)
            {
                return;
            }

            FileInfo zipFile = Constants.DownloadsDirectory.File("sde.zip");

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

            File.WriteAllText(doneFile.FullName, "");
        }
    }
}
