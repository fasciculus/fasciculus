using Fasciculus.Net.Navigating;
using Fasciculus.Site.Models;
using Fasciculus.Site.Navigation;
using Fasciculus.Site.Specifications.Models;

namespace Fasciculus.Site.Specifications.Services
{
    public class SpecificationNavigation
    {
        private readonly SpecificationContent content;

        public SpecificationNavigation(SpecificationContent content)
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

            foreach (SpecificationPackage package in content.GetPackages())
            {
                UriPath packageLink = new("specifications", package.Name);
                bool isOpen = packageLink.IsSelfOrAncestorOf(selected);

                NavigationNode packageNode = new(NavigationKind.SpecificationPackage, package.Name, packageLink, isOpen);

                foreach (SpecificationEntry entry in package.GetEntries())
                {
                    UriPath entryLink = new("specifications", package.Name, entry.Id);
                    NavigationNode entryNode = new(NavigationKind.SpecificationEntry, entry.Title, entryLink);

                    packageNode.Add(entryNode);
                }

                forest.Add(packageNode);
            }

            return forest;
        }
    }
}
