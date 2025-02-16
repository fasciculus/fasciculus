using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Site.Specifications.Models
{
    public class SpecificationPackages
    {
        private readonly Dictionary<string, SpecificationPackage> packages = [];

        public SpecificationPackages(IEnumerable<SpecificationEntry> entries)
        {
            foreach (SpecificationEntry entry in entries)
            {
                Add(entry);
            }
        }

        public IEnumerable<SpecificationPackage> GetPackages()
        {
            return packages.Values.OrderBy(p => p.Name);
        }

        public SpecificationPackage GetPackage(string packageName)
        {
            return packages[packageName];
        }

        public void Add(SpecificationEntry entry)
        {
            SpecificationPackage? package = null;

            if (!packages.TryGetValue(entry.Package, out package))
            {
                package = new SpecificationPackage(entry.Package);
                packages[entry.Package] = package;
            }

            package.Add(entry);
        }
    }
}
