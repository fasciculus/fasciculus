using System.Collections;
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

    public class SdeNames : IEnumerable<SdeName>
    {
        private readonly List<SdeName> names;
        private readonly Dictionary<int, string> namesById = [];

        public SdeNames(SdeName[] names)
        {
            this.names = new(names);
            names.Apply(name => { namesById[name.ItemID] = name.ItemName; });
        }

        public string this[int id]
        {
            get { return namesById.TryGetValue(id, out string? name) ? name : string.Empty; }
        }

        public IEnumerator<SdeName> GetEnumerator()
            => names.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => names.GetEnumerator();
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
