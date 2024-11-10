using System.Collections.Generic;

namespace Fasciculus.Eve.Models
{
    public class LocalizedName
    {
        public string En { get; set; } = string.Empty;

        public static readonly LocalizedName Empty = new() { En = string.Empty };
    }

    public class SdeName
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; } = string.Empty;
    }

    public class SdeNames
    {
        private readonly Dictionary<int, string> names = [];

        public SdeNames(IEnumerable<SdeName> names)
        {
            names.Apply(name => { this.names[name.ItemID] = name.ItemName; });
        }

        public string this[int id]
        {
            get { return names.TryGetValue(id, out string? name) ? name : string.Empty; }
        }
    }

    public class SdeType
    {
        public int GroupId { get; set; }
        public int MarketGroupID { get; set; }
        public double Mass { get; set; }
        public LocalizedName Name { get; set; } = LocalizedName.Empty;
        public int PortionSize { get; set; }
        public bool Published { get; set; }
        public double Volume { get; set; } = double.MaxValue;
    }

    public class SdeTypes
    {
        private Dictionary<int, SdeType> types;

        public SdeTypes(Dictionary<int, SdeType> types)
        {
            this.types = types;
        }
    }

    public class SdeStationOperation
    {
        public int[] Services { get; set; } = [];
    }

    public class SdeStationOperations
    {
        private Dictionary<int, SdeStationOperation> stationOperations;

        public SdeStationOperations(Dictionary<int, SdeStationOperation> stationOperations)
        {
            this.stationOperations = stationOperations;
        }
    }

    public class SdeData
    {
        public required SdeNames Names { get; init; }
        public required SdeStationOperations StationOperations { get; init; }
        public required SdeTypes Types { get; set; }
    }
}
