using Fasciculus.Net.Navigating;
using System.Collections;
using System.Collections.Generic;

namespace Fasciculus.Site.Navigation
{
    public class SitePath : IEnumerable<SitePathEntry>
    {
        private readonly List<SitePathEntry> entries = [];

        public int Count => entries.Count;

        public void Add(SitePathEntry entry)
            => entries.Add(entry);

        public void Add(int kind, string label, UriPath link)
            => Add(SitePathEntry.Create(kind, label, link));

        public IEnumerator<SitePathEntry> GetEnumerator()
            => entries.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => entries.GetEnumerator();
    }
}
