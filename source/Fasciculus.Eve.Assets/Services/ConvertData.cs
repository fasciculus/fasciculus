using Fasciculus.Eve.Assets.Models;
using Fasciculus.Eve.Models;
using Fasciculus.Maui.Support;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IConvertData
    {
        public Task<EveData.Data> Data { get; }
    }

    public class ConvertData : IConvertData
    {
        private readonly IParseData parseData;
        private readonly IAssetsProgress progress;

        private EveData.Data? data;
        private readonly TaskSafeMutex dataMutex = new();

        public Task<EveData.Data> Data => GetDataAsync();

        public ConvertData(IParseData parseData, IAssetsProgress progress)
        {
            this.parseData = parseData;
            this.progress = progress;
        }

        private async Task<EveData.Data> GetDataAsync()
        {
            using Locker locker = Locker.Lock(dataMutex);

            if (data is null)
            {
                SdeData sdeData = await parseData.Data;

                progress.ConvertDataProgress.Report(WorkState.Working);

                DateTime version = sdeData.Version;
                EveMarketGroup.Data[] marketGroups = ConvertMarketGroups(sdeData.MarketGroups);
                EveType.Data[] types = ConvertTypes(sdeData.Types);
                EveStationOperation.Data[] stationOperations = ConvertStationOperations(sdeData.StationOperations);
                EveNpcCorporation.Data[] npcCorporations = ConvertNpcCorporations(sdeData.NpcCorporations);
                EvePlanetSchematic.Data[] planetSchematics = ConvertPlanetSchematics(sdeData.PlanetSchematics);
                EveBlueprint.Data[] blueprints = ConvertBlueprints(sdeData.Blueprints);

                data = new(version, marketGroups, types, stationOperations, npcCorporations, planetSchematics, blueprints);

                progress.ConvertDataProgress.Report(WorkState.Done);
            }

            return data;
        }

        private static EveMarketGroup.Data[] ConvertMarketGroups(Dictionary<int, SdeMarketGroup> marketGroups)
            => [.. marketGroups.Select(ConvertMarketGroup).OrderBy(t => t.Id)];

        private static EveMarketGroup.Data ConvertMarketGroup(KeyValuePair<int, SdeMarketGroup> kvp)
        {
            SdeMarketGroup marketGroup = kvp.Value;

            int id = kvp.Key;
            string name = marketGroup.NameID.En;
            int parentId = marketGroup.ParentGroupID;

            return new(id, name, parentId);
        }

        private static EveType.Data[] ConvertTypes(Dictionary<int, SdeType> types)
            => [.. types.Select(ConvertType).OrderBy(t => t.Id)];

        private static EveType.Data ConvertType(KeyValuePair<int, SdeType> kvp)
        {
            SdeType sdeType = kvp.Value;

            int id = kvp.Key;
            string name = sdeType.Name.En;
            double volume = sdeType.Volume;
            int marketGroupId = sdeType.MarketGroupID;

            return new(id, name, volume, marketGroupId);
        }

        private static EveStationOperation.Data[] ConvertStationOperations(Dictionary<int, SdeStationOperation> stationOperations)
            => [.. stationOperations.Select(ConvertStationOperation).OrderBy(t => t.Id)];

        private static EveStationOperation.Data ConvertStationOperation(KeyValuePair<int, SdeStationOperation> kvp)
        {
            SdeStationOperation sdeStationOperation = kvp.Value;

            int id = kvp.Key;
            string name = sdeStationOperation.OperationNameID.En;

            return new(id, name);
        }

        private static EveNpcCorporation.Data[] ConvertNpcCorporations(Dictionary<int, SdeNpcCorporation> npcCorporations)
            => [.. npcCorporations.Select(ConvertNpcCorporation).OrderBy(t => t.Id)];

        private static EveNpcCorporation.Data ConvertNpcCorporation(KeyValuePair<int, SdeNpcCorporation> kvp)
        {
            SdeNpcCorporation sdeNpcCorporation = kvp.Value;

            int id = kvp.Key;
            string name = sdeNpcCorporation.NameID.En;

            return new(id, name);
        }

        private static EvePlanetSchematic.Data[] ConvertPlanetSchematics(Dictionary<int, SdePlanetSchematic> planetSchematics)
            => [.. planetSchematics.Select(ConvertPlanetSchematic).OrderBy(t => t.Id)];

        private static EvePlanetSchematic.Data ConvertPlanetSchematic(KeyValuePair<int, SdePlanetSchematic> kvp)
        {
            SdePlanetSchematic sdePlanetSchematic = kvp.Value;

            int id = kvp.Key;
            string name = sdePlanetSchematic.NameID.En;
            int cycleTime = sdePlanetSchematic.CycleTime;

            EvePlanetSchematicType.Data[] inputs = [.. sdePlanetSchematic.Types
                .Where(x => x.Value.IsInput)
                .Select(ConvertPlanetSchematicType)
                .OrderBy(x => x.Type)];

            EvePlanetSchematicType.Data output = ConvertPlanetSchematicType(sdePlanetSchematic.Types.Single(x => !x.Value.IsInput));

            return new(id, name, cycleTime, inputs, output);
        }

        private static EvePlanetSchematicType.Data ConvertPlanetSchematicType(KeyValuePair<int, SdePlanetSchematicType> kvp)
        {
            SdePlanetSchematicType sdePlanetSchematicType = kvp.Value;

            int id = kvp.Key;
            int quantity = sdePlanetSchematicType.Quantity;

            return new(id, quantity);
        }

        private static EveBlueprint.Data[] ConvertBlueprints(Dictionary<int, SdeBlueprint> blueprints)
            => [.. blueprints.Select(ConvertBlueprint)/*.OrderBy(t => t.Id)*/];

        private static EveBlueprint.Data ConvertBlueprint(KeyValuePair<int, SdeBlueprint> kvp)
        {
            SdeBlueprint sdeBlueprint = kvp.Value;

            return new();
        }
    }

    public static class ConvertDataServices
    {
        public static IServiceCollection AddConvertData(this IServiceCollection services)
        {
            services.AddParseData();
            services.AddAssetsProgress();

            services.TryAddSingleton<IConvertData, ConvertData>();

            return services;
        }
    }
}
