using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Site.Specifications.Models
{
    public class SpecificationPackage
    {
        private readonly Dictionary<string, SpecificationEntry> entries = [];

        public string Name { get; }

        public SpecificationPackage(string name)
        {
            Name = name;
        }

        public IEnumerable<SpecificationEntry> GetEntries()
        {
            return entries.Values.OrderBy(e => e.Title);
        }

        public SpecificationEntry GetEntry(string id)
        {
            return entries[id];
        }

        public void Add(SpecificationEntry entry)
        {
            entries[entry.Id] = entry;
        }
    }
}
