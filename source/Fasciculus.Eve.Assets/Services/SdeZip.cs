using Fasciculus.IO;
using Fasciculus.Net;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IDownloadSde
    {
        public Task<FileInfo> DownloadedFile { get; }
    }

    public class DownloadSde : IDownloadSde
    {
        public static readonly string SdeZipUri = "https://eve-static-data-export.s3-eu-west-1.amazonaws.com/tranquility/sde.zip";

        private readonly IDownloader downloader;
        private readonly IAssetsDirectories assetsDirectories;
        private readonly IAssetsProgress progress;

        private FileInfo? downloadedFile;
        private readonly TaskSafeMutex mutex = new();

        public Task<FileInfo> DownloadedFile => GetDownloadedFileAsync();

        public DownloadSde(IDownloader downloader, IAssetsDirectories assetsDirectories, IAssetsProgress progress)
        {
            this.downloader = downloader;
            this.assetsDirectories = assetsDirectories;
            this.progress = progress;
        }

        private async Task<FileInfo> GetDownloadedFileAsync()
        {
            using Locker locker = Locker.Lock(mutex);

            if (downloadedFile is null)
            {
                progress.DownloadSde.Report(DownloadSdeStatus.Downloading);

                FileInfo destination = assetsDirectories.Downloads.File("sde.zip");
                DownloaderResult result = await downloader.DownloadAsync(new(SdeZipUri), destination);

                downloadedFile = result.DownloadedFile;

                progress.DownloadSde.Report(result.NotModified ? DownloadSdeStatus.NotModified : DownloadSdeStatus.Downloaded);
            }

            return downloadedFile;
        }
    }

    public class SdeFiles
    {
        private readonly DirectoryInfo sdeDirectory;

        public FileInfo NamesYaml
            => sdeDirectory.Combine("bsd").File("invNames.yaml");

        public FileInfo TypesYaml
            => sdeDirectory.Combine("fsd").File("types.yaml");

        public FileInfo MarketGroupYaml
            => sdeDirectory.Combine("fsd").File("marketGroups.yaml");

        public FileInfo StationOperationsYaml
            => sdeDirectory.Combine("fsd").File("stationOperations.yaml");

        public FileInfo NpcCorporationsYaml
            => sdeDirectory.Combine("fsd").File("npcCorporations.yaml");

        public DirectoryInfo[] Regions
            => sdeDirectory.Combine("universe", "eve").GetDirectories();

        public SdeFiles(DirectoryInfo sdeDirectory)
        {
            this.sdeDirectory = sdeDirectory;
        }
    }

    public interface IExtractSde
    {
        public Task<SdeFiles> Files { get; }
    }

    public class ExtractSde : IExtractSde
    {
        private readonly IDownloadSde downloadSde;
        private readonly IAssetsDirectories assetsDirectories;
        private readonly ICompression compression;
        private readonly IAssetsProgress progress;

        private SdeFiles? files = null;
        private readonly TaskSafeMutex mutex = new();

        public Task<SdeFiles> Files => GetFiles();

        public ExtractSde(IDownloadSde downloadSde, IAssetsDirectories assetsDirectories, ICompression compression,
            IAssetsProgress progress)
        {
            this.downloadSde = downloadSde;
            this.assetsDirectories = assetsDirectories;
            this.compression = compression;
            this.progress = progress;
        }

        private async Task<SdeFiles> GetFiles()
        {
            using Locker locker = Locker.Lock(mutex);

            if (files is null)
            {
                FileInfo file = await downloadSde.DownloadedFile;
                DirectoryInfo directory = compression.Unzip(file, assetsDirectories.Sde, FileOverwriteMode.IfNewer, progress.ExtractSde);

                files = new(directory);
            }

            return files;
        }
    }

    public static class SdeZipServices
    {
        public static IServiceCollection AddSdeZip(this IServiceCollection services)
        {
            services.AddAssetsDirectories();
            services.AddAssetsProgress();

            services.AddDownloader();
            services.AddCompression();

            services.TryAddSingleton<IDownloadSde, DownloadSde>();
            services.TryAddSingleton<IExtractSde, ExtractSde>();

            return services;
        }
    }
}