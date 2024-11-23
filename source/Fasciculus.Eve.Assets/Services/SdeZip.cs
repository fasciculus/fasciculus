using Fasciculus.IO;
using Fasciculus.Net;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Assets.Services
{
    public enum DownloadSdeStatus
    {
        Pending,
        Downloading,
        Downloaded,
        NotModified
    }

    public interface IDownloadSde
    {
        public Task<FileInfo> DownloadAsync();
    }

    public class DownloadSde : IDownloadSde
    {
        public static readonly string SdeZipUri = "https://eve-static-data-export.s3-eu-west-1.amazonaws.com/tranquility/sde.zip";

        private readonly IDownloader downloader;
        private readonly IAssetsFiles assetFiles;
        private readonly IProgress<DownloadSdeStatus> progress;

        private Tuple<FileInfo, bool>? result;
        private readonly TaskSafeMutex resultMutex = new();

        public DownloadSde(IDownloader downloader, IAssetsFiles assetFiles, IProgress<DownloadSdeStatus> progress)
        {
            this.downloader = downloader;
            this.assetFiles = assetFiles;
            this.progress = progress;
        }

        public async Task<FileInfo> DownloadAsync()
        {
            using Locker locker = Locker.Lock(resultMutex);

            if (result == null)
            {
                progress.Report(DownloadSdeStatus.Downloading);

                result = await downloader.DownloadAsync(new(SdeZipUri), assetFiles.SdeZip);

                progress.Report(result.Item2 ? DownloadSdeStatus.NotModified : DownloadSdeStatus.Downloaded);
            }

            return result.Item1;
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
                FileInfo file = downloadSde.DownloadAsync().Run();

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
    }
}