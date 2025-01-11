using Fasciculus.Site.Models;

namespace Fasciculus.Site.Blog.Models
{
    public class BlogEntryViewModel : ViewModel
    {
        public required BlogEntry Entry { get; init; }
    }
}
