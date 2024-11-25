namespace Fasciculus.Eve.Assets.Models
{
    public class SdeLocalized
    {
        public string En { get; set; } = string.Empty;

        public static SdeLocalized Empty = new();
    }

    public class SdeName
    {
        public long ItemID { get; set; }
        public string ItemName { get; set; } = string.Empty;
    }

    public class SdeType
    {
        public long GroupID { get; set; }
        public SdeLocalized Name { get; set; } = SdeLocalized.Empty;
        public double Volume { get; set; } = double.MaxValue;
    }

    public class SdeData
    {
        public Dictionary<long, string> Names { get; init; } = [];
        public Dictionary<long, SdeType> Types { get; init; } = [];
    }
}
