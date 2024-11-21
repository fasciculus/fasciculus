using Fasciculus.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Fasciculus.IO
{
    public readonly struct UnzipProgressMessage
    {
        public FileInfo? ExtractedFile { get; init; }

        public long TotalCompressed { get; init; }
        public long TotalUncompressed { get; init; }

        public long CurrentCompressed { get; init; }
        public long CurrentUncompressed { get; init; }
    }

    public interface ICompression
    {
        public void Unzip(FileInfo zipFile, DirectoryInfo outputDirectory, FileOverwriteMode overwrite, IProgress<UnzipProgressMessage> progress);
    }

    public class Compression : ICompression
    {
        private readonly struct EntryProgressMessage
        {
            public FileInfo ExtractedFile { get; init; }
            public ZipArchiveEntry ExtractedEntry { get; init; }
        }

        private class EntryProgress : TaskSafeProgress<EntryProgressMessage>
        {
            private readonly IProgress<UnzipProgressMessage> progress;

            private readonly long totalCompressed;
            private readonly long totalUncompressed;

            private long currentCompressed = 0;
            private long currentUncompressed = 0;

            public EntryProgress(IProgress<UnzipProgressMessage> progress, long totalCompressed, long totalUncompressed)
            {
                this.progress = progress;
                this.totalCompressed = totalCompressed;
                this.totalUncompressed = totalUncompressed;
            }

            protected override void Process(EntryProgressMessage value)
            {
                currentCompressed += value.ExtractedEntry.CompressedLength;
                currentUncompressed += value.ExtractedEntry.Length;

                UnzipProgressMessage message = new()
                {
                    ExtractedFile = value.ExtractedFile,
                    TotalCompressed = totalCompressed,
                    TotalUncompressed = totalUncompressed,
                    CurrentCompressed = currentCompressed,
                    CurrentUncompressed = currentUncompressed
                };

                progress.Report(message);
            }

            public void ReportStart()
            {
                UnzipProgressMessage message = new()
                {
                    TotalCompressed = totalCompressed,
                    TotalUncompressed = totalUncompressed,
                };

                progress.Report(message);
            }
        }

        public void Unzip(FileInfo zipFile, DirectoryInfo outputDirectory, FileOverwriteMode overwrite, IProgress<UnzipProgressMessage> progress)
        {
            using Stream stream = zipFile.OpenRead();
            using ZipArchive archive = new(stream, ZipArchiveMode.Read);

            ZipArchiveEntry[] entries = archive.Entries.Where(entry => IsUnzipRequired(entry, outputDirectory, overwrite)).ToArray();
            long totalCompressed = entries.Sum(entry => entry.CompressedLength);
            long totalUncompressed = entries.Sum(entry => entry.Length);
            EntryProgress entryProgress = new(progress, totalCompressed, totalUncompressed);

            entryProgress.ReportStart();

            entries.Apply(entry => UnzipEntry(archive, entry.FullName, outputDirectory, entryProgress));

            entryProgress.Done();
        }

        private void UnzipEntry(ZipArchive archive, string entryFullName, DirectoryInfo outputDirectory, IProgress<EntryProgressMessage> progress)
        {
            ZipArchiveEntry entry = archive.GetEntry(entryFullName);
            FileInfo outputFile = PrepareUnzipOutputFile(entryFullName, outputDirectory);

            entry.ExtractToFile(outputFile.FullName);
            outputFile.CreationTimeUtc = outputFile.LastWriteTimeUtc = entry.LastWriteTime.UtcDateTime;

            EntryProgressMessage message = new()
            {
                ExtractedFile = new(outputFile.FullName),
                ExtractedEntry = entry
            };

            progress.Report(message);
        }

        private FileInfo PrepareUnzipOutputFile(string entryFullName, DirectoryInfo outputDirectory)
        {
            FileInfo outputFile = outputDirectory.File(entryFullName);

            outputFile.Directory.Create();
            outputFile.DeleteIfExists();

            return outputFile;
        }

        private bool IsUnzipRequired(ZipArchiveEntry entry, DirectoryInfo outputDirectory, FileOverwriteMode overwrite)
        {
            FileInfo file = outputDirectory.File(entry.FullName);
            DateTime dateTime = entry.LastWriteTime.UtcDateTime;

            return file.NeedsOverwrite(dateTime, overwrite);
        }
    }

    public static class CompressionServices
    {
        public static IServiceCollection AddCompression(this IServiceCollection services)
        {
            services.TryAddSingleton<ICompression, Compression>();

            return services;
        }

        public static HostApplicationBuilder UseCompression(this HostApplicationBuilder builder)
        {
            builder.Services.AddCompression();

            return builder;
        }
    }

    public static class Zip
    {
        public enum Overwrite
        {
            Never,
            Always,
            Changed
        }

        public static async Task ExtractAsync(FileInfo zipFile, DirectoryInfo outputDirectory, Overwrite overwrite)
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

        public static void Extract(FileInfo zipFile, DirectoryInfo outputDirectory, Overwrite overwrite)
            => ExtractAsync(zipFile, outputDirectory, overwrite).Run();

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

    public static class GZip
    {
        public static void Compress(Stream uncompressed, Stream compressed)
        {
            using GZipStream gzStream = new(compressed, CompressionMode.Compress);

            uncompressed.CopyTo(gzStream);
        }

        public static void Compress(Stream uncompressed, FileInfo compressedFile)
        {
            compressedFile.Write(stream => Compress(uncompressed, stream));
        }

        public static void Extract(Stream compressed, Stream uncompressed)
        {
            using GZipStream gzStream = new(compressed, CompressionMode.Decompress);

            gzStream.CopyTo(uncompressed);
        }

        public static void Extract(FileInfo compressedFile, Stream uncompressed)
        {
            using Stream compressed = compressedFile.OpenRead();

            Extract(compressed, uncompressed);
        }

        public static byte[] Extract(FileInfo compressedFile)
        {
            MemoryStream uncompressed = new();

            Extract(compressedFile, uncompressed);

            return uncompressed.ToArray();
        }
    }

}