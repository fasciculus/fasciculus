using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net;
using System.Linq;

namespace Fasciculus.ApiDoc.Models
{
    public class ApiClass : ApiElement
    {
        public override UriPath Link { get; }

        public Modifiers Modifiers { get; }

        public ApiClass(ClassInfo @class, ApiNamespace @namespace)
            : base(@class)
        {
            Link = @namespace.Link.Append(CreateLinkPart(@class.UntypedName, @class.Parameters.Count()));
            Modifiers = @class.Modifiers;
        }
    }
}
