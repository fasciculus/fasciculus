using System;

namespace Fasciculus.Site.Blog.Models
{
    public class BlogFrontMatter
    {
        public string Title { get; set; } = string.Empty;
        public DateTime Published { get; set; } = DateTime.MinValue;
        public string Author { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
    }
}
