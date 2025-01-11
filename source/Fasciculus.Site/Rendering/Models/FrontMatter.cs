using System.Collections;
using System.Collections.Generic;

namespace Fasciculus.Site.Rendering.Models
{
    public abstract class FrontMatter : IFrontMatter, IEnumerable<FrontMatterEntry>
    {
        public abstract IEnumerable<FrontMatterEntry> GetEntries();

        public IEnumerator<FrontMatterEntry> GetEnumerator()
            => GetEntries().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEntries().GetEnumerator();
    }
}
