using Fasciculus.IO;
using Fasciculus.IO.Compressing;
using Fasciculus.Net;
using Fasciculus.Support.Progressing;
using Fasciculus.Threading.Synchronization;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IDownloadSde
    {
        public Task<FileInfo> DownloadedFile { get; }
    }

    public class DownloadSde : IDownloadSde
    {
        public static readonly string SdeZipUri = "https://eve-static-data-export.s3-eu-west-1.amazonaws.com/tranquility/sde.zip";

        private readonly IAssetsDirectories assetsDirectories;
        private readonly IAccumulatingProgress<long> progress;

        private FileInfo? downloadedFile;
        private readonly TaskSafeMutex mutex = new();

        public Task<FileInfo> DownloadedFile => GetDownloadedFileAsync();

        public DownloadSde(IAssetsDirectories assetsDirectories, IAssetsProgress progress)
        {
            this.assetsDirectories = assetsDirectories;
            this.progress = progress.DownloadSde;
        }

        private async Task<FileInfo> GetDownloadedFileAsync()
        {
            using Locker locker = Locker.Lock(mutex);

            if (downloadedFile is null)
            {
                progress.Begin(2);
                progress.Report(1);

                FileInfo destination = assetsDirectories.Downloads.File("sde.zip");
                HttpClient httpClient = HttpClientFactory.Create(null);
                DownloaderResult result = await Downloader.DownloadAsync(httpClient, new(SdeZipUri), destination);

                downloadedFile = result.DownloadedFile;

                progress.End();
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

        public FileInfo PlanetSchematicsYaml
            => sdeDirectory.Combine("fsd").File("planetSchematics.yaml");

        public FileInfo BlueprintsYaml
            => sdeDirectory.Combine("fsd").File("blueprints.yaml");

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
        private readonly IAccumulatingProgress<long> progress;

        private SdeFiles? files = null;
        private readonly TaskSafeMutex mutex = new();

        public Task<SdeFiles> Files => GetFiles();

        public ExtractSde(IDownloadSde downloadSde, IAssetsDirectories assetsDirectories, IAssetsProgress progress)
        {
            this.downloadSde = downloadSde;
            this.assetsDirectories = assetsDirectories;
            this.progress = progress.ExtractSde;
        }

        private async Task<SdeFiles> GetFiles()
        {
            using Locker locker = Locker.Lock(mutex);

            if (files is null)
            {
                FileInfo file = await downloadSde.DownloadedFile;
                DirectoryInfo directory = Zip.Extract(file, assetsDirectories.Sde, FileOverwriteMode.IfNewer, progress);

                files = new(directory);
            }

            return files;
        }
    }
}