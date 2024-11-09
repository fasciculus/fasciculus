using System.Collections.Generic;

namespace Fasciculus.Eve.Models
{
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

    public class SdeData
    {
        public readonly SdeNames Names;

        public SdeData(SdeNames names)
        {
            Names = names;
        }
    }
}
