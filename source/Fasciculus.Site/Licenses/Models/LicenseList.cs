using Fasciculus.Collections;
using Microsoft.CodeAnalysis;
using NuGet.Protocol.Core.Types;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Site.Licenses.Models
{
    public class LicenseList : IEnumerable<LicenseEntry>
    {
        public string Package { get; }

        private readonly SortedSet<LicenseEntry> entries;

        public int Count => entries.Count;

        public SortedSet<string> Licenses => new(entries.Select(e => e.License));

        public LicenseList(string package)
        {
            Package = package;

            entries = [];
        }

        private LicenseList(LicenseList other, bool _)
        {
            Package = other.Package;
            entries = new(other.entries);
        }

        public LicenseList Clone()
            => new(this, true);

        public void AddOrMergeWith(IPackageSearchMetadata[] metadatas)
            => metadatas.Apply(Add);

        private void Add(IPackageSearchMetadata metadata)
        {
            if (!entries.Any(e => e.Identity.Equals(metadata.Identity)))
            {
                entries.Add(LicenseEntry.Create(metadata));
            }
        }

        public IEnumerator<LicenseEntry> GetEnumerator()
            => entries.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => entries.GetEnumerator();
    }
}
