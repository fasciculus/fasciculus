using Fasciculus.Net.Navigating;
using Fasciculus.Support;

namespace Fasciculus.Site.Models
{
    public class SiteDocument
    {
        public required string Title { get; init; }

        private NavigationForest? navigation = null;

        public bool HasNavigation => navigation is not null;

        public NavigationForest Navigation
        {
            get => Cond.NotNull(navigation);
            set { navigation = value; }
        }
    }
}
