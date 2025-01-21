using Fasciculus.Site.Navigation;
using Fasciculus.Support;

namespace Fasciculus.Site.Models
{
    public class ViewModel
    {
        public required string Title { get; init; }

        private SiteNavigation? navigation = null;

        public bool HasNavigation => navigation is not null;

        public SiteNavigation Navigation
        {
            get => Cond.NotNull(navigation);
            set { navigation = value; }
        }
    }
}
