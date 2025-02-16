namespace Fasciculus.Site.Specifications.Models
{
    public class SpecificationEntry
    {
        public required string Package { get; init; }

        public required string Id { get; init; }

        public required string Title { get; init; }

        public required string Html { get; init; }
    }
}
