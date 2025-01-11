using Fasciculus.Site.Models;
using System.Collections.Generic;

namespace Fasciculus.Site.Blog.Models
{
    public class BlogViewModel : ViewModel
    {
        public required List<BlogEntry> Entries { get; init; }
    }
}
