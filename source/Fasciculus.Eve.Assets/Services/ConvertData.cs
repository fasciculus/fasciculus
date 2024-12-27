using Fasciculus.Eve.Assets.Models;
using Fasciculus.Eve.Models;
using Fasciculus.Threading.Synchronization;

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

                progress.ConvertData.Begin(2);
                progress.ConvertData.Report(1);

                DateTime version = sdeData.Version;
                EveMarketGroup.Data[] marketGroups = ConvertMarketGroups(sdeData.MarketGroups);
                EveType.Data[] types = ConvertTypes(sdeData.Types);
                EveStationOperation.Data[] stationOperations = ConvertStationOperations(sdeData.StationOperations);
                EveNpcCorporation.Data[] npcCorporations = ConvertNpcCorporations(sdeData.NpcCorporations);
                EvePlanetSchematic.Data[] planetSchematics = ConvertPlanetSchematics(sdeData.PlanetSchematics);
                EveBlueprint.Data[] blueprints = ConvertBlueprints(sdeData.Blueprints);

                SortedSet<int> typeIds = new(types.Select(x => x.Id));

                blueprints = [.. blueprints.Where(x => IsValid(x, typeIds))];

                data = new(version, marketGroups, types, stationOperations, npcCorporations, planetSchematics, blueprints);

                progress.ConvertData.End();
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
            int metaGroupId = (int)sdeType.MetaGroupId;
            int marketGroupId = sdeType.MarketGroupID;

            return new(id, name, volume, metaGroupId, marketGroupId);
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

            EvePlanetTypeQuantity.Data[] inputs = [.. sdePlanetSchematic.Types
                .Where(x => x.Value.IsInput)
                .Select(ConvertPlanetSchematicType)
                .OrderBy(x => x.Type)];

            EvePlanetTypeQuantity.Data output = ConvertPlanetSchematicType(sdePlanetSchematic.Types.Single(x => !x.Value.IsInput));

            return new(id, name, cycleTime, inputs, output);
        }

        private static EvePlanetTypeQuantity.Data ConvertPlanetSchematicType(KeyValuePair<int, SdePlanetSchematicType> kvp)
        {
            SdePlanetSchematicType sdePlanetSchematicType = kvp.Value;

            int id = kvp.Key;
            int quantity = sdePlanetSchematicType.Quantity;

            return new(id, quantity);
        }

        private static EveBlueprint.Data[] ConvertBlueprints(Dictionary<int, SdeBlueprint> blueprints)
            => [.. blueprints.Select(ConvertBlueprint).OrderBy(t => t.Id)];

        private static EveBlueprint.Data ConvertBlueprint(KeyValuePair<int, SdeBlueprint> kvp)
        {
            SdeBlueprint blueprint = kvp.Value;

            int id = kvp.Key;
            int maxRuns = blueprint.MaxProductionLimit;
            EveManufacturing.Data manufacturing = ConvertManufacturing(blueprint.Activities.Manufacturing);

            return new(id, maxRuns, manufacturing);
        }

        private static EveManufacturing.Data ConvertManufacturing(SdeManufacturing manufacturing)
        {
            int time = manufacturing.Time;
            EveMaterial.Data[] materials = ConvertMaterials(manufacturing.Materials);
            EveMaterial.Data[] products = ConvertMaterials(manufacturing.Products);
            EveSkill.Data[] skills = ConvertSkills(manufacturing.Skills);

            return new(time, materials, products, skills);
        }

        private static EveMaterial.Data[] ConvertMaterials(IEnumerable<SdeMaterial> materials)
            => [.. materials.Select(ConvertMaterial).OrderBy(x => x.Type)];


        private static EveMaterial.Data ConvertMaterial(SdeMaterial material)
        {
            int type = material.TypeID;
            int quantity = material.Quantity;

            return new(type, quantity);
        }

        private static EveSkill.Data[] ConvertSkills(IEnumerable<SdeSkill> skills)
            => [.. skills.Select(ConvertSkill).OrderBy(x => x.Id)];

        private static EveSkill.Data ConvertSkill(SdeSkill skill)
        {
            int id = skill.TypeID;
            int level = skill.Level;

            return new(id, level);
        }

        private static bool IsValid(EveBlueprint.Data blueprint, SortedSet<int> types)
        {
            EveManufacturing.Data manufacturing = blueprint.Manufacturing;

            if (!types.Contains(blueprint.Id))
            {
                return false;
            }

            if (manufacturing.Time < 1)
            {
                return false;
            }

            if (manufacturing.Materials.Any(x => !types.Contains(x.Type)))
            {
                return false;
            }

            if (manufacturing.Products.Any(x => !types.Contains(x.Type)))
            {
                return false;
            }

            if (manufacturing.Skills.Any(x => !types.Contains(x.Id)))
            {
                return false;
            }

            return true;
        }
    }
}
