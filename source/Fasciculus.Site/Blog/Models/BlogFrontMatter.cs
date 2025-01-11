using Fasciculus.Site.Rendering.Models;
using System;
using System.Collections.Generic;

namespace Fasciculus.Site.Blog.Models
{
    public class BlogFrontMatter : FrontMatter
    {
        public string Title { get; set; } = string.Empty;
        public DateTime Published { get; set; } = DateTime.MinValue;
        public string Author { get; set; } = string.Empty;

        public override IEnumerable<FrontMatterEntry> GetEntries()
        {
            yield return new FrontMatterEntry("Published", Published.ToString("yyyy-MM-dd"));
            yield return new FrontMatterEntry("Author", Author);
        }
    }
}
