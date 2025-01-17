using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Site.Models;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Site.Api.Models
{
    public class ApiNamespaceViewModel : ViewModel
    {
        public required NamespaceSymbol Namespace { get; init; }

        public IEnumerable<ClassSymbol> Classes => Namespace.Classes;
        public bool HasClasses => Classes.Any();

        public IEnumerable<EnumSymbol> Enums => Namespace.Enums;
        public bool HasEnums => Enums.Any();
    }
}
