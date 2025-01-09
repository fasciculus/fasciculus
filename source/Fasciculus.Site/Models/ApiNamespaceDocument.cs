using Fasciculus.CodeAnalysis.Models;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Site.Models
{
    public class ApiNamespaceDocument : SiteDocument
    {
        public required NamespaceSymbol Namespace { get; init; }

        public IEnumerable<ClassSymbol> Classes => Namespace.Classes;
        public bool HasClasses => Classes.Any();
    }
}
