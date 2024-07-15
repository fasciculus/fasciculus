using Fasciculus.Eve.IO;
using Fasciculus.IO;
using System;
using System.IO;
using System.IO.Compression;

namespace Fasciculus.Eve
{
    public static class SDE
    {
        public static FileInfo Compressed => EveDirectories.Downloads.File("sde.zip");
        public static DirectoryInfo Extracted => EveDirectories.Data.Combine("SDE").Existing();

        public static void Extract()
        {
            using Stream stream = new FileStream(Compressed.FullName, FileMode.Open, FileAccess.Read);
            using ZipArchive zip = new(stream);

            foreach (ZipArchiveEntry entry in zip.Entries)
            {
                Console.WriteLine(entry.FullName);
            }
        }
    }
}
