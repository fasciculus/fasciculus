using Fasciculus.CodeAnalysis.Frameworks;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net;

namespace Fasciculus.ApiDoc.Models
{
    public abstract class ApiElement
    {
        public string Name { get; }

        public TargetFrameworks Frameworks { get; }

        public abstract UriPath Link { get; }

        public ApiElement(ElementInfo element)
        {
            Name = element.Name;
            Frameworks = element.Frameworks;
        }

        protected static string CreateLinkPart(string part, int parameterCount = 0)
            => parameterCount > 0 ? $"{part}-{parameterCount}" : part;
    }
}
