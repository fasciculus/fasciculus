using Fasciculus.Eve.Assets.Models;
using Fasciculus.Maui.Support;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IParseData
    {
        public Task<DateTime> Version { get; }
        public Task<Dictionary<int, string>> Names { get; }
        public Task<Dictionary<int, SdeMarketGroup>> MarketGroups { get; }
        public Task<Dictionary<int, SdeType>> Types { get; }
        public Task<Dictionary<int, SdeStationOperation>> StationOperations { get; }
        public Task<Dictionary<int, SdeNpcCorporation>> NpcCorporations { get; }

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

        private Dictionary<int, string>? names = null;
        private readonly TaskSafeMutex namesMutex = new();

        private Dictionary<int, SdeMarketGroup>? marketGroups = null;
        private readonly TaskSafeMutex marketGroupsMutex = new();

        private Dictionary<int, SdeType>? types = null;
        private readonly TaskSafeMutex typesMutex = new();

        private Dictionary<int, SdeStationOperation>? stationOperations = null;
        private readonly TaskSafeMutex stationOperationsMutex = new();

        private Dictionary<int, SdeNpcCorporation>? npcCorporations = null;
        private readonly TaskSafeMutex npcCorporationsMutex = new();

        private SdeData? data = null;
        private readonly TaskSafeMutex dataMutex = new();

        public Task<DateTime> Version => GetVersionAsync();
        public Task<Dictionary<int, string>> Names => GetNamesAsync();
        public Task<Dictionary<int, SdeMarketGroup>> MarketGroups => GetMarketGroupsAsync();
        public Task<Dictionary<int, SdeType>> Types => GetTypesAsync();
        public Task<Dictionary<int, SdeStationOperation>> StationOperations => GetStationOperationsAsync();
        public Task<Dictionary<int, SdeNpcCorporation>> NpcCorporations => GetNpcCorporationsAsync();

        public Task<SdeData> Data => GetDataAsync();

        public ParseData(IDownloadSde downloadSde, IExtractSde extractSde, IYaml yaml, IAssetsProgress progress)
        {
            this.downloadSde = downloadSde;
            this.extractSde = extractSde;
            this.yaml = yaml;
            this.progress = progress;
        }

        private async Task<DateTime> GetVersionAsync()
        {
            using Locker locker = Locker.Lock(versionMutex);

            if (!version.HasValue)
            {
                FileInfo downloadedFile = await downloadSde.DownloadedFile;

                version = downloadedFile.LastWriteTimeUtc;
            }

            return version.Value;
        }

        private Task<Dictionary<int, string>> GetNamesAsync()
        {
            SdeFiles sdeFiles = Tasks.Wait(extractSde.Files);

            return Tasks.LongRunning(() => GetNames(sdeFiles));
        }

        private Dictionary<int, string> GetNames(SdeFiles sdeFiles)
        {
            using Locker locker = Locker.Lock(namesMutex);

            if (names is null)
            {
                progress.ParseNamesProgress.Report(WorkState.Working);

                names = yaml
                    .Deserialize<SdeName[]>(sdeFiles.NamesYaml)
                    .ToDictionary(n => n.ItemID, n => n.ItemName);

                progress.ParseNamesProgress.Report(WorkState.Done);
            }

            return names;
        }

        private Task<Dictionary<int, SdeMarketGroup>> GetMarketGroupsAsync()
        {
            SdeFiles sdeFiles = Tasks.Wait(extractSde.Files);

            return Tasks.LongRunning(() => GetMarketGroups(sdeFiles));
        }

        private Dictionary<int, SdeMarketGroup> GetMarketGroups(SdeFiles sdeFiles)
        {
            using Locker locker = Locker.Lock(marketGroupsMutex);

            if (marketGroups is null)
            {
                progress.ParseMarketGroupsProgress.Report(PendingToDone.Working);
                marketGroups = yaml.Deserialize<Dictionary<int, SdeMarketGroup>>(sdeFiles.MarketGroupYaml);
                progress.ParseMarketGroupsProgress.Report(PendingToDone.Done);
            }

            return marketGroups;
        }

        private Task<Dictionary<int, SdeType>> GetTypesAsync()
        {
            SdeFiles sdeFiles = Tasks.Wait(extractSde.Files);

            return Tasks.LongRunning(() => GetTypes(sdeFiles));
        }

        private Dictionary<int, SdeType> GetTypes(SdeFiles sdeFiles)
        {
            using Locker locker = Locker.Lock(typesMutex);

            if (types is null)
            {
                progress.ParseTypesProgress.Report(PendingToDone.Working);
                types = yaml.Deserialize<Dictionary<int, SdeType>>(sdeFiles.TypesYaml);
                progress.ParseTypesProgress.Report(PendingToDone.Done);
            }

            return types;
        }

        private Task<Dictionary<int, SdeStationOperation>> GetStationOperationsAsync()
        {
            SdeFiles sdeFiles = Tasks.Wait(extractSde.Files);

            return Tasks.Start(() => GetStationOperations(sdeFiles));
        }

        private Dictionary<int, SdeStationOperation> GetStationOperations(SdeFiles sdeFiles)
        {
            using Locker locker = Locker.Lock(stationOperationsMutex);

            if (stationOperations is null)
            {
                progress.ParseStationOperationsProgress.Report(PendingToDone.Working);
                stationOperations = yaml.Deserialize<Dictionary<int, SdeStationOperation>>(sdeFiles.StationOperationsYaml);
                progress.ParseStationOperationsProgress.Report(PendingToDone.Done);
            }

            return stationOperations;
        }

        private Task<Dictionary<int, SdeNpcCorporation>> GetNpcCorporationsAsync()
        {
            SdeFiles sdeFiles = Tasks.Wait(extractSde.Files);

            return Tasks.Start(() => GetNpcCorporations(sdeFiles));
        }

        private Dictionary<int, SdeNpcCorporation> GetNpcCorporations(SdeFiles sdeFiles)
        {
            using Locker locker = Locker.Lock(npcCorporationsMutex);

            if (npcCorporations is null)
            {
                progress.ParseNpcCorporationsProgress.Report(PendingToDone.Working);
                npcCorporations = yaml.Deserialize<Dictionary<int, SdeNpcCorporation>>(sdeFiles.NpcCorporationsYaml);
                progress.ParseNpcCorporationsProgress.Report(PendingToDone.Done);
            }

            return npcCorporations;
        }

        private Task<SdeData> GetDataAsync()
        {
            return Tasks.LongRunning(GetData);
        }

        private SdeData GetData()
        {
            using Locker locker = Locker.Lock(dataMutex);

            if (data is null)
            {
                Task.WaitAll([Version, Names, MarketGroups, Types, StationOperations, NpcCorporations]);

                data = new()
                {
                    Version = Version.Result,
                    Names = Names.Result,
                    MarketGroups = MarketGroups.Result,
                    Types = Types.Result,
                    StationOperations = StationOperations.Result,
                    NpcCorporations = NpcCorporations.Result
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
