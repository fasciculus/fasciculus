using Fasciculus.CodeAnalysis.Frameworks;
using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.ApiDoc.Models
{
    public class ApiElement
    {
        public string Name { get; }

        public TargetFrameworks Frameworks { get; }

        public ApiElement(ElementInfo element)
        {
            Name = element.Name;
            Frameworks = element.Frameworks;
        }
    }
}
