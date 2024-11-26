using Fasciculus.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Fasciculus.IO
{
    public interface ICompression
    {
        public void Unzip(FileInfo zipFile, DirectoryInfo outputDirectory, FileOverwriteMode overwrite, ILongProgress progress);

        public void GZip(Stream uncompressed, Stream compressed);
        public void UnGZip(Stream compressed, Stream uncompressed);
    }

    public class Compression : ICompression
    {
        public void Unzip(FileInfo zipFile, DirectoryInfo outputDirectory, FileOverwriteMode overwrite, ILongProgress progress)
        {
            using Stream stream = zipFile.OpenRead();
            using ZipArchive archive = new(stream, ZipArchiveMode.Read);

            ZipArchiveEntry[] entries = archive.Entries.Where(entry => IsUnzipRequired(entry, outputDirectory, overwrite)).ToArray();
            long total = entries.Sum(entry => entry.Length);

            progress.Start(total);

            entries.Apply(entry => UnzipEntry(archive, entry.FullName, outputDirectory, progress));

            progress.Done();
        }

        private void UnzipEntry(ZipArchive archive, string entryFullName, DirectoryInfo outputDirectory, ILongProgress progress)
        {
            ZipArchiveEntry entry = archive.GetEntry(entryFullName);
            FileInfo outputFile = PrepareUnzipOutputFile(entryFullName, outputDirectory);

            entry.ExtractToFile(outputFile.FullName);
            outputFile.CreationTimeUtc = outputFile.LastWriteTimeUtc = entry.LastWriteTime.UtcDateTime;

            progress.Report(entry.Length);
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

        public void GZip(Stream uncompressed, Stream compressed)
        {
            using GZipStream gzStream = new(compressed, CompressionMode.Compress);

            uncompressed.CopyTo(gzStream);
        }

        public void UnGZip(Stream compressed, Stream uncompressed)
        {
            using GZipStream gzStream = new(compressed, CompressionMode.Decompress);

            gzStream.CopyTo(uncompressed);
        }
    }

    public static class CompressionServices
    {
        public static IServiceCollection AddCompression(this IServiceCollection services)
        {
            services.TryAddSingleton<ICompression, Compression>();

            return services;
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