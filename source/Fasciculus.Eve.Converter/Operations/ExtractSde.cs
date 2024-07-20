using System.IO;
using System.IO.Compression;

namespace Fasciculus.Eve.Operations
{
    public static class ExtractSde
    {
        public static FileInfo SdeFile
            => Constants.DownloadsDirectory.File("sde.zip");

        public static DirectoryInfo TargetDirectory
            => Constants.SdeDirectory;

        public static void Run()
        {
            using Stream zipStream = new FileStream(SdeFile.FullName, FileMode.Open, FileAccess.Read);
            using ZipArchive zip = new(zipStream);

            foreach (ZipArchiveEntry entry in zip.Entries)
            {
                FileInfo targetFile = TargetDirectory.File(entry.FullName);

                if (targetFile.Exists)
                {
                    continue;
                }

                targetFile.Directory?.Existing();

                using Stream inStream = entry.Open();
                using Stream outStream = targetFile.Create();

                inStream.CopyTo(outStream);
            }
        }
    }
}
