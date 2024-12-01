using Fasciculus.Eve.Assets.Models;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IParseData
    {
        public Task<DateTime> Version { get; }
        public Task<Dictionary<long, string>> Names { get; }
        public Task<Dictionary<long, SdeType>> Types { get; }

        public Task<SdeData> Data { get; }
    }

    public class ParseData : IParseData
    {
        private readonly IDownloadSde downloadSde;
        private readonly IExtractSde extractSde;
        private readonly IYaml yaml;
        private readonly IAssetsProgress progress;

        private DateTime? version = null;
        private readonly TaskSafeMutex versionMutex = new();

        private Dictionary<long, string>? names = null;
        private readonly TaskSafeMutex namesMutex = new();

        private Dictionary<long, SdeType>? types = null;
        private readonly TaskSafeMutex typesMutex = new();

        private SdeData? data = null;
        private readonly TaskSafeMutex dataMutex = new();

        public Task<DateTime> Version => GetVersion();
        public Task<Dictionary<long, string>> Names => GetNames();
        public Task<Dictionary<long, SdeType>> Types => GetTypes();

        public Task<SdeData> Data => GetData();

        public ParseData(IDownloadSde downloadSde, IExtractSde extractSde, IYaml yaml, IAssetsProgress progress)
        {
            this.downloadSde = downloadSde;
            this.extractSde = extractSde;
            this.yaml = yaml;
            this.progress = progress;
        }

        private async Task<DateTime> GetVersion()
        {
            using Locker locker = Locker.Lock(versionMutex);

            if (!version.HasValue)
            {
                FileInfo downloadedFile = await downloadSde.DownloadedFile;

                version = downloadedFile.LastWriteTimeUtc;
            }

            return version.Value;
        }

        private async Task<Dictionary<long, string>> GetNames()
        {
            using Locker locker = Locker.Lock(namesMutex);

            if (names is null)
            {
                progress.ParseNames.Report(PendingToDone.Working);

                SdeFiles sdeFiles = await extractSde.Files;

                names = yaml
                    .Deserialize<SdeName[]>(sdeFiles.NamesYaml)
                    .ToDictionary(n => n.ItemID, n => n.ItemName);

                progress.ParseNames.Report(PendingToDone.Done);
            }

            return names;
        }

        private async Task<Dictionary<long, SdeType>> GetTypes()
        {
            using Locker locker = Locker.Lock(typesMutex);

            if (types is null)
            {
                progress.ParseTypes.Report(PendingToDone.Working);

                SdeFiles sdeFiles = await extractSde.Files;

                types = yaml.Deserialize<Dictionary<long, SdeType>>(sdeFiles.TypesYaml);

                progress.ParseTypes.Report(PendingToDone.Done);
            }

            return types;
        }

        private async Task<SdeData> GetData()
        {
            using Locker locker = Locker.Lock(dataMutex);

            if (data is null)
            {
                Task.WaitAll([Version, Names, Types]);
                await Task.Yield();

                data = new()
                {
                    Version = Version.Result,
                    Names = Names.Result,
                    Types = Types.Result,
                };
            }

            return data;
        }
    }

    public static class ParseDataServices
    {
        public static IServiceCollection AddParseData(this IServiceCollection services)
        {
            services.AddAssetsDirectories();
            services.AddAssetsProgress();

            services.AddSdeZip();
            services.AddYaml();

            services.TryAddSingleton<IParseData, ParseData>();

            return services;
        }
    }
}
