using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
        private readonly Dictionary<int, string> names;

        public ReadOnlyDictionary<int, string> Names
            => names.AsReadOnly();

        public SdeNames(SdeName[] names)
        {
            this.names = names.ToDictionary(n => n.ItemID, n => n.ItemName);
        }

        public string this[int id]
        {
            get { return names.TryGetValue(id, out string? name) ? name : string.Empty; }
        }
    }

    public class SdeNpcCorporation
    {
        public LocalizedName NameId { get; set; } = LocalizedName.Empty;
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

    public class SdeStationOperation
    {
        public LocalizedName OperationNameID { get; set; } = LocalizedName.Empty;
        public int[] Services { get; set; } = [];
    }

    public class SdeData
    {
        public required SdeNames Names { get; init; }
        public required Dictionary<int, SdeNpcCorporation> NpcCorporations { get; init; }
        public required Dictionary<int, SdeStationOperation> StationOperations { get; init; }
        public required Dictionary<int, SdeType> Types { get; set; }
    }
}
