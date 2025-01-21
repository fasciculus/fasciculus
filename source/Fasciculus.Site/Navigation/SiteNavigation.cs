using Fasciculus.Net.Navigating;

namespace Fasciculus.Site.Navigation
{
    public class SiteNavigation
    {
        public required NavigationForest Forest { get; init; }

        public required SitePath Path { get; init; }
    }
}
