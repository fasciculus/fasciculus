using Fasciculus.Support;
using Fasciculus.Threading;
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
        public DirectoryInfo Unzip(FileInfo file, DirectoryInfo target, FileOverwriteMode overwrite, IAccumulatingLongProgress progress);

        public void GZip(Stream uncompressed, Stream compressed);
        public void UnGZip(Stream compressed, Stream uncompressed);
    }

    public class Compression : ICompression
    {
        public DirectoryInfo Unzip(FileInfo file, DirectoryInfo target, FileOverwriteMode overwrite, IAccumulatingLongProgress progress)
        {
            using Stream stream = file.OpenRead();
            using ZipArchive archive = new(stream, ZipArchiveMode.Read);

            ZipArchiveEntry[] entries = archive.Entries.Where(entry => IsUnzipRequired(entry, target, overwrite)).ToArray();
            long total = entries.Sum(entry => entry.Length);

            progress.Begin(total);
            entries.Apply(entry => UnzipEntry(archive, entry.FullName, target, progress));
            progress.End();

            return new(target.FullName);
        }

        private void UnzipEntry(ZipArchive archive, string name, DirectoryInfo target, IAccumulatingLongProgress progress)
        {
            ZipArchiveEntry entry = archive.GetEntry(name);
            FileInfo outputFile = PrepareUnzipTarget(name, target);

            entry.ExtractToFile(outputFile.FullName);
            outputFile.CreationTimeUtc = outputFile.LastWriteTimeUtc = entry.LastWriteTime.UtcDateTime;

            progress.Report(entry.Length);
        }

        private FileInfo PrepareUnzipTarget(string name, DirectoryInfo target)
        {
            FileInfo outputFile = target.File(name);

            outputFile.Directory.Create();
            outputFile.DeleteIfExists();

            return outputFile;
        }

        private bool IsUnzipRequired(ZipArchiveEntry entry, DirectoryInfo target, FileOverwriteMode overwrite)
        {
            FileInfo file = target.File(entry.FullName);
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
            => Tasks.Wait(ExtractAsync(zipFile, outputDirectory, overwrite));

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