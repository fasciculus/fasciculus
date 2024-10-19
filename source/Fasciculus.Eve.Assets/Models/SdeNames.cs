using System.Collections.Generic;

namespace Fasciculus.Eve.Models
{
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
}
