using Fasciculus.Collections;
using Fasciculus.Progress;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Fasciculus.IO.Compressing
{
    /// <summary>
    /// Support for zip files.
    /// </summary>
    public static class Zip
    {
        /// <summary>
        /// Extracts the given <paramref name="file"/> into the given <paramref name="target"/> directory.
        /// </summary>
        public static DirectoryInfo Extract(FileInfo file, DirectoryInfo target, FileOverwrite overwrite)
            => Extract(file, target, overwrite, null, null);

        /// <summary>
        /// Extracts the given <paramref name="file"/> into the given <paramref name="target"/> directory.
        /// </summary>
        public static DirectoryInfo Extract(FileInfo file, DirectoryInfo target, FileOverwrite overwrite,
            IAccumulatingProgress<long> lengthProgress)
            => Extract(file, target, overwrite, lengthProgress, null);

        /// <summary>
        /// Extracts the given <paramref name="file"/> into the given <paramref name="target"/> directory.
        /// </summary>
        public static DirectoryInfo Extract(FileInfo file, DirectoryInfo target, FileOverwrite overwrite,
            IProgress<string> nameProgress)
            => Extract(file, target, overwrite, null, nameProgress);

        /// <summary>
        /// Extracts the given <paramref name="file"/> into the given <paramref name="target"/> directory.
        /// </summary>
        public static DirectoryInfo Extract(FileInfo file, DirectoryInfo target, FileOverwrite overwrite,
            IAccumulatingProgress<long>? lengthProgress, IProgress<string>? nameProgress)
        {
            using Stream stream = file.OpenRead();
            using ZipArchive archive = new(stream, ZipArchiveMode.Read);
            ReadOnlyCollection<ZipArchiveEntry> entries = archive.Entries;

            ReportBegin(entries, lengthProgress, nameProgress);

            entries.Apply(entry => { Extract(entry, target, overwrite, lengthProgress, nameProgress); });

            ReportEnd(lengthProgress, nameProgress);

            return target;
        }

        private static void Extract(ZipArchiveEntry entry, DirectoryInfo target, FileOverwrite overwrite,
            IAccumulatingProgress<long>? lengthProgress, IProgress<string>? nameProgress)
        {
            nameProgress?.Report(entry.FullName);

            if (IsExtractRequired(entry, target, overwrite))
            {
                FileInfo outputFile = target.File(entry.FullName);

                outputFile.Directory?.Create();
                outputFile.DeleteIfExists();

                entry.ExtractToFile(outputFile.FullName);
                outputFile.CreationTimeUtc = outputFile.LastWriteTimeUtc = entry.LastWriteTime.UtcDateTime;
            }

            lengthProgress?.Report(entry.Length);
        }

        private static bool IsExtractRequired(ZipArchiveEntry entry, DirectoryInfo target, FileOverwrite overwrite)
        {
            FileInfo file = target.File(entry.FullName);
            DateTime dateTime = entry.LastWriteTime.UtcDateTime;

            return file.RequiresOverwrite(dateTime, overwrite);
        }

        private static void ReportBegin(ReadOnlyCollection<ZipArchiveEntry> entries,
            IAccumulatingProgress<long>? lengthProgress, IProgress<string>? nameProgress)
        {
            if (lengthProgress is not null)
            {
                long totalLength = entries.Sum(entry => entry.Length);

                lengthProgress.Begin(totalLength);
            }

            nameProgress?.Report(string.Empty);
        }

        private static void ReportEnd(IAccumulatingProgress<long>? lengthProgress, IProgress<string>? nameProgress)
        {
            lengthProgress?.End();
            nameProgress?.Report(string.Empty);
        }
    }
}
