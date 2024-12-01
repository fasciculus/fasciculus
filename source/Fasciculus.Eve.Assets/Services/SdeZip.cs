﻿using Fasciculus.IO;
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
            await Task.Yield();

            using Locker locker = Locker.Lock(mutex);

            if (downloadedFile is null)
            {
                progress.DownloadSde.Report(DownloadSdeStatus.Downloading);

                FileInfo destination = assetsDirectories.Downloads.File("sde.zip");

                downloadedFile = downloader.Download(new(SdeZipUri), destination, out bool notModified);

                progress.DownloadSde.Report(notModified ? DownloadSdeStatus.NotModified : DownloadSdeStatus.Downloaded);
            }

            return downloadedFile;
        }
    }

    public interface ISdeFileSystem
    {
        public FileInfo NamesYaml { get; }
        public FileInfo TypesYaml { get; }

        public DirectoryInfo[] Regions { get; }
    }

    public class SdeFileSystem : ISdeFileSystem
    {
        private readonly DirectoryInfo sdeDirectory;

        public FileInfo NamesYaml
            => sdeDirectory.Combine("bsd").File("invNames.yaml");

        public FileInfo TypesYaml
            => sdeDirectory.Combine("fsd").File("types.yaml");

        public DirectoryInfo[] Regions
            => sdeDirectory.Combine("universe", "eve").GetDirectories();

        public SdeFileSystem(DirectoryInfo sdeDirectory)
        {
            this.sdeDirectory = sdeDirectory;
        }
    }

    public interface IExtractSde
    {
        public ISdeFileSystem Extract();
    }

    public class ExtractSde : IExtractSde
    {
        private readonly IDownloadSde downloadSde;
        private readonly IAssetsDirectories assetsDirectories;
        private readonly ICompression compression;
        private readonly IAssetsProgress progress;

        private ISdeFileSystem? result = null;
        private readonly TaskSafeMutex resultMutex = new();

        public ExtractSde(IDownloadSde downloadSde, IAssetsDirectories assetsDirectories, ICompression compression,
            IAssetsProgress progress)
        {
            this.downloadSde = downloadSde;
            this.assetsDirectories = assetsDirectories;
            this.compression = compression;
            this.progress = progress;
        }

        public ISdeFileSystem Extract()
        {
            using Locker locker = Locker.Lock(resultMutex);

            if (result is null)
            {
                FileInfo file = Tasks.Wait(downloadSde.DownloadedFile);

                compression.Unzip(file, assetsDirectories.Sde, FileOverwriteMode.IfNewer, progress.ExtractSde);
                result = new SdeFileSystem(assetsDirectories.Sde);
            }

            return result;
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