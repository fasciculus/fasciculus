using System.Collections.Generic;

namespace Fasciculus.Site.Rendering.Models
{
    public interface IFrontMatter
    {
        public IEnumerable<FrontMatterEntry> GetEntries();
    }
}
