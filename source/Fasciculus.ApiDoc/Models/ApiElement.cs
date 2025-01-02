using System.Collections.Generic;

namespace Fasciculus.ApiDoc.Models
{
    public class ApiElement
    {
        public required string Name { get; init; }

        public SortedSet<string> TargetFrameworks { get; } = [];
    }
}
