using Fasciculus.Eve.Assets.Models;
using Fasciculus.Maui.Support;
using Fasciculus.Threading;
using Fasciculus.Threading.Synchronization;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IParseData
    {
        public Task<DateTime> Version { get; }
        public Task<Dictionary<uint, string>> Names { get; }
        public Task<Dictionary<int, SdeMarketGroup>> MarketGroups { get; }
        public Task<Dictionary<int, SdeType>> Types { get; }
        public Task<Dictionary<int, SdeStationOperation>> StationOperations { get; }
        public Task<Dictionary<int, SdeNpcCorporation>> NpcCorporations { get; }
        public Task<Dictionary<int, SdePlanetSchematic>> PlanetSchematics { get; }
        public Task<Dictionary<int, SdeBlueprint>> Blueprints { get; }

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

        private Dictionary<uint, string>? names = null;
        private readonly TaskSafeMutex namesMutex = new();

        private Dictionary<int, SdeMarketGroup>? marketGroups = null;
        private readonly TaskSafeMutex marketGroupsMutex = new();

        private Dictionary<int, SdeType>? types = null;
        private readonly TaskSafeMutex typesMutex = new();

        private Dictionary<int, SdeStationOperation>? stationOperations = null;
        private readonly TaskSafeMutex stationOperationsMutex = new();

        private Dictionary<int, SdeNpcCorporation>? npcCorporations = null;
        private readonly TaskSafeMutex npcCorporationsMutex = new();

        private Dictionary<int, SdePlanetSchematic>? planetSchematics = null;
        private readonly TaskSafeMutex planetSchematicsMutex = new();

        private Dictionary<int, SdeBlueprint>? blueprints = null;
        private readonly TaskSafeMutex blueprintsMutex = new();

        private SdeData? data = null;
        private readonly TaskSafeMutex dataMutex = new();

        public Task<DateTime> Version => GetVersionAsync();
        public Task<Dictionary<uint, string>> Names => GetNamesAsync();
        public Task<Dictionary<int, SdeMarketGroup>> MarketGroups => GetMarketGroupsAsync();
        public Task<Dictionary<int, SdeType>> Types => GetTypesAsync();
        public Task<Dictionary<int, SdeStationOperation>> StationOperations => GetStationOperationsAsync();
        public Task<Dictionary<int, SdeNpcCorporation>> NpcCorporations => GetNpcCorporationsAsync();
        public Task<Dictionary<int, SdePlanetSchematic>> PlanetSchematics => GetPlanetSchematicsAsync();
        public Task<Dictionary<int, SdeBlueprint>> Blueprints => GetBlueprintsAsync();

        public Task<SdeData> Data => GetDataAsync();

        public ParseData(IDownloadSde downloadSde, IExtractSde extractSde, IYaml yaml, IAssetsProgress progress)
        {
            this.downloadSde = downloadSde;
            this.extractSde = extractSde;
            this.yaml = yaml;
            this.progress = progress;
        }

        private SdeFiles GetSdeFiles()
        {
            return Tasks.Wait(extractSde.Files);
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

        private Task<Dictionary<uint, string>> GetNamesAsync()
        {
            using Locker locker = Locker.Lock(namesMutex);

            return Tasks.LongRunning(() => GetNames(GetSdeFiles()));
        }

        private Dictionary<uint, string> GetNames(SdeFiles sdeFiles)
        {
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
            using Locker locker = Locker.Lock(marketGroupsMutex);

            return Tasks.LongRunning(() => GetMarketGroups(GetSdeFiles()));
        }

        private Dictionary<int, SdeMarketGroup> GetMarketGroups(SdeFiles sdeFiles)
        {
            if (marketGroups is null)
            {
                progress.ParseMarketGroupsProgress.Report(WorkState.Working);
                marketGroups = yaml.Deserialize<Dictionary<int, SdeMarketGroup>>(sdeFiles.MarketGroupYaml);
                progress.ParseMarketGroupsProgress.Report(WorkState.Done);
            }

            return marketGroups;
        }

        private Task<Dictionary<int, SdeType>> GetTypesAsync()
        {
            using Locker locker = Locker.Lock(typesMutex);

            return Tasks.LongRunning(() => GetTypes(GetSdeFiles()));
        }

        private Dictionary<int, SdeType> GetTypes(SdeFiles sdeFiles)
        {
            if (types is null)
            {
                progress.ParseTypesProgress.Report(WorkState.Working);
                types = yaml.Deserialize<Dictionary<int, SdeType>>(sdeFiles.TypesYaml);
                progress.ParseTypesProgress.Report(WorkState.Done);
            }

            return types;
        }

        private Task<Dictionary<int, SdeStationOperation>> GetStationOperationsAsync()
        {
            using Locker locker = Locker.Lock(stationOperationsMutex);

            return Tasks.Start(() => GetStationOperations(GetSdeFiles()));
        }

        private Dictionary<int, SdeStationOperation> GetStationOperations(SdeFiles sdeFiles)
        {
            if (stationOperations is null)
            {
                progress.ParseStationOperationsProgress.Report(WorkState.Working);
                stationOperations = yaml.Deserialize<Dictionary<int, SdeStationOperation>>(sdeFiles.StationOperationsYaml);
                progress.ParseStationOperationsProgress.Report(WorkState.Done);
            }

            return stationOperations;
        }

        private Task<Dictionary<int, SdeNpcCorporation>> GetNpcCorporationsAsync()
        {
            using Locker locker = Locker.Lock(npcCorporationsMutex);

            return Tasks.Start(() => GetNpcCorporations(GetSdeFiles()));
        }

        private Dictionary<int, SdeNpcCorporation> GetNpcCorporations(SdeFiles sdeFiles)
        {
            if (npcCorporations is null)
            {
                progress.ParseNpcCorporationsProgress.Report(WorkState.Working);
                npcCorporations = yaml.Deserialize<Dictionary<int, SdeNpcCorporation>>(sdeFiles.NpcCorporationsYaml);
                progress.ParseNpcCorporationsProgress.Report(WorkState.Done);
            }

            return npcCorporations;
        }

        private Task<Dictionary<int, SdePlanetSchematic>> GetPlanetSchematicsAsync()
        {
            using Locker locker = Locker.Lock(planetSchematicsMutex);

            return Tasks.Start(() => GetPlanetSchematics(GetSdeFiles()));
        }

        private Dictionary<int, SdePlanetSchematic> GetPlanetSchematics(SdeFiles sdeFiles)
        {
            if (planetSchematics is null)
            {
                progress.ParsePlanetSchematicsProgress.Report(WorkState.Working);
                planetSchematics = yaml.Deserialize<Dictionary<int, SdePlanetSchematic>>(sdeFiles.PlanetSchematicsYaml);
                progress.ParsePlanetSchematicsProgress.Report(WorkState.Done);
            }

            return planetSchematics;
        }

        private Task<Dictionary<int, SdeBlueprint>> GetBlueprintsAsync()
        {
            using Locker locker = Locker.Lock(blueprintsMutex);

            return Tasks.Start(() => GetBlueprints(GetSdeFiles()));
        }

        private Dictionary<int, SdeBlueprint> GetBlueprints(SdeFiles sdeFiles)
        {
            if (blueprints is null)
            {
                progress.ParseBlueprintsProgress.Report(WorkState.Working);
                blueprints = yaml.Deserialize<Dictionary<int, SdeBlueprint>>(sdeFiles.BlueprintsYaml);
                progress.ParseBlueprintsProgress.Report(WorkState.Done);
            }

            return blueprints;
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
                Task.WaitAll(
                [
                    Version,
                    Names,
                    MarketGroups,
                    Types,
                    StationOperations,
                    NpcCorporations,
                    PlanetSchematics,
                    Blueprints,
                ]);

                data = new()
                {
                    Version = Version.Result,
                    Names = Names.Result,
                    MarketGroups = MarketGroups.Result,
                    Types = Types.Result,
                    StationOperations = StationOperations.Result,
                    NpcCorporations = NpcCorporations.Result,
                    PlanetSchematics = PlanetSchematics.Result,
                    Blueprints = Blueprints.Result,
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
