﻿using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Fasciculus.IO
{
    public static class Zip
    {
        public enum Overwrite
        {
            Never,
            Always,
            Changed
        }

        public static async Task Extract(FileInfo zipFile, DirectoryInfo outputDirectory, Overwrite overwrite)
        {
            using Stream stream = zipFile.OpenRead();
            using ZipArchive archive = new(stream, ZipArchiveMode.Read);

            IEnumerable<ZipArchiveEntry> entries = archive.Entries.Where(e => Filter(e, outputDirectory, overwrite));

            foreach (ZipArchiveEntry entry in entries)
            {
                FileInfo outputFile = outputDirectory.File(entry.FullName);

                outputFile.DeleteIfExists();
                outputFile.Directory.Create();

                using Stream entryStream = entry.Open();
                using Stream outputStream = outputFile.Create();

                await entryStream.CopyToAsync(outputStream);
            }
        }

        private static bool Filter(ZipArchiveEntry entry, DirectoryInfo outputDirectory, Overwrite overwrite)
        {
            if (overwrite == Overwrite.Always) return true;

            FileInfo outputFile = outputDirectory.File(entry.FullName);

            if (outputFile.Exists)
            {
                if (overwrite == Overwrite.Never) return false;

                if (entry.LastWriteTime.UtcDateTime > outputFile.LastWriteTimeUtc) return true;
                if (entry.Length != outputFile.Length) return true;

                return false;
            }

            return true;
        }
    }
}
