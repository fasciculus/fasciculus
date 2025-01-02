using System.Collections.Generic;

namespace Fasciculus.ApiDoc.Models
{
    public class ApiPackage
    {
        public required string Name { get; init; }
        public string Description { get; set; } = string.Empty;

        public List<string> TargetFrameworks { get; } = [];
    }
}
