using Fasciculus.Net.Navigating;
using Fasciculus.Site.Models;
using Fasciculus.Site.Navigation;

namespace Fasciculus.Site.Releases.Services
{
    public class ReleasesNavigation
    {
        private readonly ReleasesContent content;

        public ReleasesNavigation(ReleasesContent content)
        {
            this.content = content;
        }

        public SiteNavigation Create(UriPath selected)
        {
            NavigationForest forest = CreateForest(selected);
            SitePath path = [];

            return new() { Forest = forest, Path = path };
        }

        private NavigationForest CreateForest(UriPath selected)
        {
            NavigationForest forest = new();

            forest.Add(new(NavigationKind.ReleasesRoadmap, "Roadmap", new("releases", "Roadmap")));

            return forest;
        }
    }
}
