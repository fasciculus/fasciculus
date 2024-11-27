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
        private readonly IAssetsDirectories assetsDirectories;
        private readonly IProgress<DownloadSdeStatus> progress;

        private FileInfo? result;
        private readonly TaskSafeMutex resultMutex = new();

        public DownloadSde(IDownloader downloader, IAssetsDirectories assetsDirectories, IProgress<DownloadSdeStatus> progress)
        {
            this.downloader = downloader;
            this.assetsDirectories = assetsDirectories;
            this.progress = progress;
        }

        public FileInfo Download()
        {
            using Locker locker = Locker.Lock(resultMutex);

            if (result == null)
            {
                progress.Report(DownloadSdeStatus.Downloading);

                FileInfo destination = assetsDirectories.Downloads.File("sde.zip");

                result = downloader.Download(new(SdeZipUri), destination, out bool notModified);

                progress.Report(notModified ? DownloadSdeStatus.NotModified : DownloadSdeStatus.Downloaded);
            }

            return result;
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
        private readonly ILongProgress progress;

        private ISdeFileSystem? result = null;
        private TaskSafeMutex resultMutex = new();

        public ExtractSde(IDownloadSde downloadSde, IAssetsDirectories assetsDirectories, ICompression compression,
            [FromKeyedServices(ServiceKeys.ExtractSde)] ILongProgress progress)
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
                FileInfo file = downloadSde.Download();

                compression.Unzip(file, assetsDirectories.Sde, FileOverwriteMode.IfNewer, progress);
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