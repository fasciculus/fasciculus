using Fasciculus.Net.Navigating;

namespace Fasciculus.Site.Navigation
{
    public class SitePathEntry
    {
        public required int Kind { get; init; }

        public required string Label { get; init; }

        public required UriPath Link { get; init; }

        public static SitePathEntry Create(int kind, string label, UriPath link)
            => new() { Kind = kind, Label = label, Link = link };
    }
}
