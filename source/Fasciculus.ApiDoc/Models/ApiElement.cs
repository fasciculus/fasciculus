using Fasciculus.CodeAnalysis.Frameworks;
using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.ApiDoc.Models
{
    public abstract class ApiElement
    {
        public string Name { get; }

        public TargetFrameworks Frameworks { get; }

        public abstract ApiLink Link { get; }

        public ApiElement(ElementInfo element)
        {
            Name = element.Name;
            Frameworks = element.Frameworks;
        }
    }
}
