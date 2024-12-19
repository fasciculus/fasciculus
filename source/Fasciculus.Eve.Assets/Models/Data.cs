namespace Fasciculus.Eve.Assets.Models
{
    public class SdeLocalized
    {
        public string En { get; set; } = string.Empty;

        public static SdeLocalized Empty = new();
    }

    public class SdeName
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; } = string.Empty;
    }

    public class SdeMarketGroup
    {
        public SdeLocalized NameID { get; set; } = SdeLocalized.Empty;
        public int ParentGroupID { get; set; }
    }

    public class SdeType
    {
        public int MarketGroupID { get; set; }
        public double MetaGroupId { get; set; } = 1;
        public SdeLocalized Name { get; set; } = SdeLocalized.Empty;
        public double Volume { get; set; } = double.MaxValue;
    }

    public class SdeStationOperation
    {
        public SdeLocalized OperationNameID { get; set; } = SdeLocalized.Empty;
    }

    public class SdeNpcCorporation
    {
        public SdeLocalized NameID { get; set; } = SdeLocalized.Empty;
    }

    public class SdePlanetSchematicType
    {
        public bool IsInput { get; set; }
        public int Quantity { get; set; }
    }

    public class SdePlanetSchematic
    {
        public int CycleTime { get; set; }
        public SdeLocalized NameID { get; set; } = SdeLocalized.Empty;
        public Dictionary<int, SdePlanetSchematicType> Types { get; set; } = [];
    }

    public class SdeMaterial
    {
        public int Quantity { get; set; }
        public int TypeID { get; set; }
    }

    public class SdeSkill
    {
        public int Level { get; set; }
        public int TypeID { get; set; }
    }

    public class SdeBlueprintCopying
    {
        public int Time { get; set; }
    }

    public class SdeBlueprintInventionProduct
    {
        public double Probability { get; set; }
        public int Quantity { get; set; }
        public int TypeID { get; set; }
    }

    public class SdeBlueprintInvention
    {
        public int Time { get; set; }

        public SdeMaterial[] Materials { get; set; } = [];
        public SdeBlueprintInventionProduct[] Products { get; set; } = [];
        public SdeSkill[] Skills { get; set; } = [];
    }

    public class SdeManufacturing
    {
        public int Time { get; set; }

        public SdeMaterial[] Materials { get; set; } = [];
        public SdeMaterial[] Products { get; set; } = [];
        public SdeSkill[] Skills { get; set; } = [];
    }

    public class SdeBlueprintResearchMaterial
    {
        public int Time { get; set; }
    }

    public class SdeBlueprintResearchTime
    {
        public int Time { get; set; }
    }

    public class SdeBlueprintActivities
    {
        public SdeBlueprintCopying Copying { get; set; } = new();
        public SdeManufacturing Manufacturing { get; set; } = new();
        public SdeBlueprintInvention? Invention { get; set; } = null;
        public SdeBlueprintResearchMaterial Research_Material { get; set; } = new();
        public SdeBlueprintResearchTime Research_Time { get; set; } = new();
    }

    public class SdeBlueprint
    {
        public int BlueprintTypeID { get; set; }
        public int MaxProductionLimit { get; set; }

        public SdeBlueprintActivities Activities { get; set; } = new();
    }

    public class SdeData
    {
        public DateTime Version { get; set; } = DateTime.MinValue;
        public Dictionary<int, string> Names { get; init; } = [];
        public Dictionary<int, SdeMarketGroup> MarketGroups { get; init; } = [];
        public Dictionary<int, SdeType> Types { get; init; } = [];
        public Dictionary<int, SdeStationOperation> StationOperations { get; init; } = [];
        public Dictionary<int, SdeNpcCorporation> NpcCorporations { get; init; } = [];
        public Dictionary<int, SdePlanetSchematic> PlanetSchematics { get; init; } = [];
        public Dictionary<int, SdeBlueprint> Blueprints { get; init; } = [];
    }
}
