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

    public class SdeType
    {
        public long GroupID { get; set; }
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

    public class SdeData
    {
        public DateTime Version { get; set; } = DateTime.MinValue;
        public Dictionary<int, string> Names { get; init; } = [];
        public Dictionary<int, SdeType> Types { get; init; } = [];
        public Dictionary<int, SdeStationOperation> StationOperations { get; init; } = [];
        public Dictionary<int, SdeNpcCorporation> NpcCorporations { get; init; } = [];
    }
}
