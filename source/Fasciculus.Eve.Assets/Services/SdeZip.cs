using Fasciculus.Eve.IO;
using Fasciculus.IO;
using Fasciculus.Net;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace Fasciculus.Eve.Services
{
    public enum DownloadSdeStatus
    {
        Downloading,
        NotModified,
        Downloaded
    }

    public readonly struct DownloadSdeMessage
    {
        public DownloadSdeStatus Status { get; init; }
    }

    public interface IDownloadSde
    {
        public FileInfo Download();
    }

    public class DownloadSde : IDownloadSde
    {
        public static readonly string SdeZipUri = "https://eve-static-data-export.s3-eu-west-1.amazonaws.com/tranquility/sde.zip";

        private readonly IDownloader downloader;
        private readonly IAssetsFiles assetFiles;
        private readonly IProgress<DownloadSdeMessage> progress;

        private FileInfo? result;
        private readonly TaskSafeMutex resultMutex = new();

        public DownloadSde(IDownloader downloader, IAssetsFiles assetFiles, IProgress<DownloadSdeMessage> progress)
        {
            this.downloader = downloader;
            this.assetFiles = assetFiles;
            this.progress = progress;
        }

        public FileInfo Download()
        {
            using Locker locker = Locker.Lock(resultMutex);

            if (result == null)
            {
                progress.Report(new() { Status = DownloadSdeStatus.Downloading });

                result = downloader.Download(new(SdeZipUri), assetFiles.SdeZip, out bool notModified);
                progress.Report(new() { Status = notModified ? DownloadSdeStatus.NotModified : DownloadSdeStatus.Downloaded });
            }

            return result;
        }
    }

    public interface IExtractSde
    {
        public bool Extract();
    }

    public class ExtractSde : IExtractSde
    {
        private readonly IDownloadSde downloadSde;
        private readonly IAssetsDirectories assetsDirectories;
        private readonly ICompression compression;
        private readonly IProgress<UnzipProgressMessage> progress;

        private bool result = false;
        private readonly TaskSafeMutex resultMutex = new();

        public ExtractSde(IDownloadSde downloadSde, IAssetsDirectories assetsDirectories, ICompression compression, IProgress<UnzipProgressMessage> progress)
        {
            this.downloadSde = downloadSde;
            this.assetsDirectories = assetsDirectories;
            this.compression = compression;
            this.progress = progress;
        }

        public bool Extract()
        {
            using Locker locker = Locker.Lock(resultMutex);

            if (!result)
            {
                FileInfo file = downloadSde.Download();

                compression.Unzip(file, assetsDirectories.Sde, FileOverwriteMode.IfNewer, progress);
                result = true;
            }

            return result;
        }
    }

    public static class SdeZipServices
    {
        public static IServiceCollection AddSdeZip(this IServiceCollection services)
        {
            services.AddDownloader();
            services.AddCompression();

            services.AddAssetsFileSystem();
            services.AddAssetsProgress();

            services.TryAddSingleton<IDownloadSde, DownloadSde>();
            services.TryAddSingleton<IExtractSde, ExtractSde>();

            return services;
        }

        public static HostApplicationBuilder UseSdeZip(this HostApplicationBuilder builder)
        {
            builder.Services.AddSdeZip();

            return builder;
        }
    }
}