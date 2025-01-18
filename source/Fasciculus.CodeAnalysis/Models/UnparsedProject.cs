using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.Net.Navigating;
using Microsoft.CodeAnalysis;

namespace Fasciculus.CodeAnalysis.Models
{
    public class UnparsedProject
    {
        public required Project Project { get; init; }

        public required UriPath RepositoryDirectory { get; init; }

        public required TargetFramework Framework { get; init; }

    }
}
