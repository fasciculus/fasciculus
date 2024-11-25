using Fasciculus.IO;
using Fasciculus.Net;
using Fasciculus.Threading;
using Fasciculus.Utilities;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IDownloadSde
    {
        public FileInfo Download();
    }

    public class DownloadSde : IDownloadSde
    {
        public static readonly string SdeZipUri = "https://eve-static-data-export.s3-eu-west-1.amazonaws.com/tranquility/sde.zip";

        private readonly IDownloader downloader;
        private readonly IAssetsFiles assetFiles;
        private readonly IProgress<DownloadSdeStatus> progress;

        private FileInfo? result;
        private readonly TaskSafeMutex resultMutex = new();

        public DownloadSde(IDownloader downloader, IAssetsFiles assetFiles, IProgress<DownloadSdeStatus> progress)
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
                progress.Report(DownloadSdeStatus.Downloading);

                result = downloader.Download(new(SdeZipUri), assetFiles.SdeZip, out bool notModified);

                progress.Report(notModified ? DownloadSdeStatus.NotModified : DownloadSdeStatus.Downloaded);
            }

            return result;
        }
    }

    public interface IExtractSde
    {
        public void Extract();
    }

    public class ExtractSde : IExtractSde
    {
        private readonly IDownloadSde downloadSde;
        private readonly IAssetsDirectories assetsDirectories;
        private readonly ICompression compression;
        private readonly ILongProgress progress;

        private bool extracted = false;
        private TaskSafeMutex extractedMutex = new();

        public ExtractSde(IDownloadSde downloadSde, IAssetsDirectories assetsDirectories, ICompression compression,
            [FromKeyedServices(ServiceKeys.ExtractSde)] ILongProgress progress)
        {
            this.downloadSde = downloadSde;
            this.assetsDirectories = assetsDirectories;
            this.compression = compression;
            this.progress = progress;
        }

        public void Extract()
        {
            using Locker locker = Locker.Lock(extractedMutex);

            if (!extracted)
            {
                FileInfo file = downloadSde.Download();

                compression.Unzip(file, assetsDirectories.Sde, FileOverwriteMode.IfNewer, progress);
                extracted = true;
            }
        }
    }

    public static class SdeZipServices
    {
        public static IServiceCollection AddSdeZip(this IServiceCollection services)
        {
            services.AddAssetsFileSystem();
            services.AddAssetsProgress();

            services.AddDownloader();
            services.AddCompression();

            services.TryAddSingleton<IDownloadSde, DownloadSde>();
            services.TryAddSingleton<IExtractSde, ExtractSde>();

            return services;
        }
    }
}