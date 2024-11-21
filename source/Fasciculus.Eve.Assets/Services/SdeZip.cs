using Fasciculus.Eve.IO;
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
        public bool Extract()
        {
            return true;
        }
    }

    public static class SdeZipServices
    {
        public static IServiceCollection AddSdeZip(this IServiceCollection services)
        {
            services.AddDownloader();
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